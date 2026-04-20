using System;
using HealthNetDb.Data;
using HealthNetDb.Entities;
using HealthNet.DTOs.LabTestDTO;
using Microsoft.EntityFrameworkCore;

namespace HealthNet.Repository.LabTestRepo;

public class LaboratoryTestingRepository : ILaboratoryTestingRepository
{
    private readonly HealthNetContext _context;

    /// <summary>
    /// Constructor for LaboratoryTestingRepository, injects HealthNetContext for database access.
    /// </summary>
    /// <param name="context">The database context used for accessing lab test data.</param>
    public LaboratoryTestingRepository(HealthNetContext context)
    {
        _context = context;
    }

    /// <summary>
    /// checks if patient exists or not
    /// </summary>
    /// <param name="patientId">The ID of the patient to check.</param>
    /// <returns>True if the patient exists, otherwise false.</returns>
    public async Task<bool> PatientExistsAsync(int patientId)
    {
        try
        {
            // Check if patient exists in the database
            return await _context.Patients
                .AnyAsync(p => p.PatientId == patientId);
        }
        catch (Exception ex)
        {
            throw new HealthNetException($"An error occurred while validating patient. {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a new labtest in DB
    /// </summary>
    /// <param name="labTest">The lab test to create.</param>
    /// <returns>The created lab test.</returns>
    /// <exception cref="HealthNetException"></exception>
    public async Task<LabTest> CreateLabTestAsync(LabTest labTest)
    {
        try
        {
            // Add new LabTest to DB and save
            await _context.LabTests.AddAsync(labTest);
            await _context.SaveChangesAsync();
            return labTest;   // TestId is now populated after save
        }
        catch (Exception ex)
        {
            throw new HealthNetException($"An error occurred while creating the lab test. {ex.Message}");
        }
    }

    public async Task<bool> TechnicianExistsAsync(int technicianId)
    {
        try
        {
            return await _context.Userss
                .Include(u => u.RoleNavigation)
                .AnyAsync(u => u.UserId == technicianId &&
                               u.RoleNavigation!.RoleName == "Lab Technician");
        }
        catch (Exception ex)
        {
            throw new HealthNetException($"An error occurred while validating technician. {ex.Message}");
        }
    }

    public async Task<bool> DuplicateTestExistsAsync(int patientId, string type)
    {
        try
        {
            return await _context.LabTests
                .AnyAsync(lt => lt.PatientId == patientId &&
                                lt.Type == type &&
                                lt.Status == false);    // false = Pending
        }
        catch (Exception ex)
        {
            throw new HealthNetException($"An error occurred while checking duplicate lab test. {ex.Message}");
        }
    }
    /// <summary>
    /// Fetches lab tests based on provided filters (Type, Status, Date).
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="statusBool"></param>
    /// <returns>A list of lab tests matching the criteria.</returns>
    /// <exception cref="HealthNetException"></exception>
    public async Task<IEnumerable<LabTest>> GetLabTestsAsync(LaboratoryTestingFilterRequest filter, bool? statusBool)
    {
        try
        {
            var query = _context.LabTests.AsQueryable();
            // Apply Type filter if provided
            if (!string.IsNullOrWhiteSpace(filter.Type))
            {
                query = query.Where(lt => lt.Type == filter.Type);
            }
            // Apply Status filter if provided
            if (statusBool.HasValue)
            {
                query = query.Where(lt => lt.Status == statusBool.Value);
            }
            // Apply Date filter if provided
            if (filter.Date.HasValue)
            {
                var date = filter.Date.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(lt => lt.Date.Date == date.Date);
            }
            return await query.ToListAsync();
        }
        catch (Exception ex)
        {
            throw new HealthNetException($"An error occurred while fetching lab tests. {ex.Message}");
        }
    }
}