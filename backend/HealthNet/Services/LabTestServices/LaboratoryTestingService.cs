using System;
using HealthNet.DTOs.LabTestDTO;
using HealthNet.Repository.LabTestRepo;
using HealthNetDb.Entities;
using HealthNetDb.Data;
using Microsoft.EntityFrameworkCore;
using HealthNet.Utility;

namespace HealthNet.Services.LabTestServices;

public class LaboratoryTestingService : ILaboratoryTestingService
{
    private readonly ILaboratoryTestingRepository _laboratoryTestingRepository;
    private readonly HealthNetContext _context;

    /// <summary>
    /// Constructor for LaboratoryTestingService, injects ILaboratoryTestingRepository for data access.
    /// </summary>
    /// <param name="laboratoryTestingRepository">The laboratory testing repository used to access lab test data.</param>
    /// <param name="context">The database context used for accessing related data.</param>
    public LaboratoryTestingService(ILaboratoryTestingRepository laboratoryTestingRepository, HealthNetContext context)
    {
        _laboratoryTestingRepository = laboratoryTestingRepository;
        _context = context;
    }

    /// <summary>
    /// Creates a new lab test for a patient.
    /// </summary>
    /// <param name="request">The request containing lab test details.</param>
    /// <returns>The response containing the created lab test details.</returns>
    /// <exception cref="HealthNetException"></exception>
    public async Task<LaboratoryTestingResponse> CreateLaboratoryTestAsync(LaboratoryTestingRequest request, int userId)
    {
        try
        {
            // Validate PatientId, return null if not found (404 in controller)
            bool patientExists = await _laboratoryTestingRepository.PatientExistsAsync(request.PatientId);
            if (!patientExists)
            {
                return null!;
            }
            // Validate TechnicianId
            bool technicianExists = await _laboratoryTestingRepository.TechnicianExistsAsync(request.TechnicianId);
            if (!technicianExists)
            {
                throw new HealthNetException($"Given ID {request.TechnicianId} is not a valid Lab Technician.");
            }
            // Check duplicate — same patient, same type, already pending
            bool duplicateExists = await _laboratoryTestingRepository.DuplicateTestExistsAsync(
                request.PatientId,
                request.Type
            );
            if (duplicateExists)
            {
                throw new HealthNetException($"A pending '{request.Type}' lab test already exists for Patient ID {request.PatientId}.");
            }
            // Validate Type using LabTestHelper
            if (!LabTestHelper.IsValidType(request.Type))
            {
                throw new HealthNetException($"Invalid test type. Must be one of: {string.Join(", ", LabTestHelper.GetValidTypes())}.");
            }

            string normalizedType = LabTestHelper.NormalizeType(request.Type);

            // Map request DTO to LabTest
            var labTest = new LabTest
            {
                PatientId = request.PatientId,
                Type = LabTestHelper.NormalizeType(request.Type),
                Date = LabTestHelper.GetUTCDateTime(), // Set to current UTC time
                TechnicianId = request.TechnicianId,
                Status = false    // false = Pending by default
            };

            // Save to DB
            var created = await _laboratoryTestingRepository.CreateLabTestAsync(labTest);

            // Fetch ActionId for "Create" action
            var actionId = await _context
                .Set<HealthNetDb.Entities.Action>()
                .Where(a => a.ActionName == "Create")
                .Select(a => a.ActionId)
                .FirstAsync();

            // Create audit log entry
            var auditLog = new AuditLog
            {
                UserId = userId,
                ActionId = actionId,
                Resource = "Lab Test",
                Timestamp = DateTime.UtcNow
            };

            // Insert audit log entry
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            // Map created entity to response DTO
            return new LaboratoryTestingResponse
            {
                TestId = created.TestId,
                PatientId = created.PatientId,
                Type = created.Type,
                Date = created.Date,
                TechnicianId = created.TechnicianId,
                Status = created.Status
            };
        }
        // Rethrowing for better error handling in controller
        catch (HealthNetException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new HealthNetException($"An error occurred while creating the lab test. {ex.Message}");
        }
    }
    public async Task<IEnumerable<LaboratoryTestingResponse>> GetLabTestsAsync(LaboratoryTestingFilterRequest filter, int userId)
    {
        try
        {
            // Validate Type if provided
            if (!string.IsNullOrWhiteSpace(filter.Type))
            {
                if (!LabTestHelper.IsValidType(filter.Type))
                {
                    throw new HealthNetException(LabTestHelper.InvalidTypeMessage);
                }
            }

            // Validate Status if provided
            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                if (!LabTestHelper.IsValidStatus(filter.Status))
                {
                    throw new HealthNetException(LabTestHelper.InvalidStatusMessage);
                }
            }

            // Validate Date if provided
            if (filter.Date.HasValue)
            {
                if (LabTestHelper.IsFutureDate(filter.Date.Value))
                {
                    throw new HealthNetException(LabTestHelper.FutureDateMessage);
                }
            }

            // Convert Status string to bool for DB query
            bool? statusBool = filter.Status switch
            {
                "Pending" => false,
                "Completed" => true,
                _ => null
            };

            // Fetch filtered results
            var labTests = await _laboratoryTestingRepository.GetLabTestsAsync(filter, statusBool);

            // Fetch ActionId for "Read" action
            var actionId = await _context
                .Set<HealthNetDb.Entities.Action>()
                .Where(a => a.ActionName == "Read")
                .Select(a => a.ActionId)
                .FirstAsync();

            var auditLog = new AuditLog
            {
                UserId = userId,
                ActionId = actionId,
                Resource = "Lab Test",
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            // Return empty list if no results — not an error
            return labTests.Select(lt => new LaboratoryTestingResponse
            {
                TestId = lt.TestId,
                PatientId = lt.PatientId,
                Type = lt.Type,
                Date = lt.Date,
                TechnicianId = lt.TechnicianId,
                Status = lt.Status
            });
        }
        catch (HealthNetException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new HealthNetException($"An error occurred while fetching lab tests. {ex.Message}");
        }
    }

    /// <summary>
    /// Updates an existing lab test. 
    /// Doctors can update both Type and TechnicianId, while Lab Technicians can only update Type. 
    /// </summary>
    /// <param name="testId"></param>
    /// <param name="request"></param>
    /// <param name="userId"></param>
    /// <param name="userRole"></param>
    /// <returns></returns>
    /// <exception cref="HealthNetException"></exception>
    public async Task<LaboratoryTestingResponse> UpdateLabTestAsync(int testId, LaboratoryTestingUpdateRequest request, int userId, string userRole)
    {
        try
        {
            // Validate at least one field is provided
            if (request.Type == null && request.TechnicianId == null)
            {
                throw new HealthNetException(LabTestHelper.NoFieldsToUpdateMessage);
            }

            // Validate TestId exists
            var labTest = await _laboratoryTestingRepository.GetLabTestByIdAsync(testId);
            if (labTest == null)
            {
                throw new HealthNetException(LabTestHelper.TestNotFoundMessage);
            }

            // Validate LabTest is Pending
            if (labTest.Status == true)
            {
                throw new HealthNetException(LabTestHelper.TestAlreadyCompletedMessage);
            }

            // Validate ownership
            if (userRole == Roles.LabTechnician && labTest.TechnicianId != userId)
            {
                throw new UnauthorizedAccessException(LabTestHelper.UnauthorizedUpdateMessage);
            }

            // Store original values BEFORE updating
            var originalType         = labTest.Type;
            var originalTechnicianId = labTest.TechnicianId;

            // Validate and update Type if provided
            if (request.Type != null)
            {
                if (!LabTestHelper.IsValidType(request.Type))
                {
                    throw new HealthNetException(LabTestHelper.InvalidTypeMessage);
                }
                labTest.Type = request.Type;
            }

            // Validate and update TechnicianId if provided and different
            if (request.TechnicianId.HasValue && request.TechnicianId.Value != labTest.TechnicianId)
            {
                // Only Doctor can reassign technician
                if (userRole != Roles.Doctor)
                {
                    throw new UnauthorizedAccessException(LabTestHelper.OnlyDoctorCanReassignMessage);
                }
                bool techExists = await _laboratoryTestingRepository.TechnicianExistsAsync(request.TechnicianId.Value);
                if (!techExists)
                {
                    throw new HealthNetException(LabTestHelper.InvalidTechnicianMessage);
                }
                labTest.TechnicianId = request.TechnicianId.Value;
            }

            // Check if anything actually changed
            if (labTest.Type == originalType && labTest.TechnicianId == originalTechnicianId)
            {
                throw new HealthNetException(LabTestHelper.NoChangesDetectedMessage);
            }

            // Save updated LabTest
            var updated = await _laboratoryTestingRepository.UpdateLabTestAsync(labTest);

            // Fetch ActionId for "Update"
            var actionId = await _context
                .Set<HealthNetDb.Entities.Action>()
                .Where(a => a.ActionName == "Update")
                .Select(a => a.ActionId)
                .FirstAsync();

            // Save AuditLog
            var auditLog = new AuditLog
            {
                UserId    = userId,
                ActionId  = actionId,
                Resource  = "Lab Test",
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            //Map entity → response DTO
            return new LaboratoryTestingResponse
            {
                TestId       = updated.TestId,
                PatientId    = updated.PatientId,
                Type         = updated.Type,
                Date         = updated.Date,
                TechnicianId = updated.TechnicianId,
                Status       = updated.Status
            };
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (HealthNetException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new HealthNetException($"An error occurred while updating lab test. {ex.Message}");
        }
    }
}