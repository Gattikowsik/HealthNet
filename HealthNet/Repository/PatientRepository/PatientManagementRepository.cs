using System;
using HealthNet.DTOs.PatientDto;
using HealthNet.DTOs.Pages;
using HealthNetDb.Data;
using HealthNetDb.Entities;
using HealthNetDb.Enums;
using Microsoft.EntityFrameworkCore;
using HealthNet.Repository.PatientRepository;

namespace HealthNet.Repository.PatientRepository;

public class PatientManagementRepository : IPatientManagementRepository
{
    private readonly HealthNetContext _context;

    public PatientManagementRepository(HealthNetContext context)
    {
        _context = context;
    }

    public async Task<PagedResponseDto<Patient>> SearchPatientsAsync(PatientSearchDto searchDto)
    {
        var query = _context.Patients.AsQueryable();

        // 🔍 Filter by Name
        if (!string.IsNullOrWhiteSpace(searchDto.Name))
        {
            query = query.Where(p => p.Name.Contains(searchDto.Name));
        }

        // 🔍 Filter by Status
        if (searchDto.Status.HasValue)
        {
            query = query.Where(p => p.Status == searchDto.Status.Value);
        }

        var totalRecords = await query.CountAsync();

        // 📄 Pagination
        var patients = await query
            .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .ToListAsync();

        return new PagedResponseDto<Patient>
        {
            TotalRecords = totalRecords,
            PageNumber = searchDto.PageNumber,
            PageSize = searchDto.PageSize,
            Items = patients
        };
    }

    public async Task<Patient> AddAsync(Patient patient)
    {
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
        return patient;
    }
}