using System;
using HealthNetDb.Data;
using HealthNetDb.Entities;
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
}