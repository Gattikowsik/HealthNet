using System;
using HealthNet.DTOs.LabTestDTO;
using HealthNet.Repository.LabTestRepo;
using HealthNetDb.Entities;

namespace HealthNet.Services.LabTestServices;

public class LabTestService : ILabTestService
{
    private readonly ILabTestRepository _labTestRepository;

    /// <summary>
    /// Constructor for LabTestService, injects ILabTestRepository for data access.
    /// </summary>
    /// <param name="labTestRepository">The lab test repository used to access lab test data.</param>
    public LabTestService(ILabTestRepository labTestRepository)
    {
        _labTestRepository = labTestRepository;
    }

    /// <summary>
    /// Creates a new lab test for a patient.
    /// </summary>
    /// <param name="request">The request containing lab test details.</param>
    /// <returns>The response containing the created lab test details.</returns>
    /// <exception cref="HealthNetException"></exception>
    public async Task<LabTestResponse> CreateLabTestAsync(LabTestRequest request)
    {
        try
        {
            // Validate PatientId, return null if not found (404 in controller)
            bool patientExists = await _labTestRepository.PatientExistsAsync(request.PatientId);
            if (!patientExists)
            {
                return null!;
            }

            // Map request DTO to LabTest
            var labTest = new LabTest
            {
                PatientId    = request.PatientId,
                Type         = request.Type,
                Date         = request.Date,
                TechnicianId = request.TechnicianId,
                Status       = false    // false = Pending by default
            };

            // Save to DB
            var created = await _labTestRepository.CreateLabTestAsync(labTest);

            // Map created entity to response DTO
            return new LabTestResponse
            {
                TestId       = created.TestId,
                PatientId    = created.PatientId,
                Type         = created.Type,
                Date         = created.Date,
                TechnicianId = created.TechnicianId,
                Status       = created.Status
            };
        }
        catch (Exception ex)
        {
            throw new HealthNetException($"An error occurred while creating the lab test. {ex.Message}");
        }
    }
}
