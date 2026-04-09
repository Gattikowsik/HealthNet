using System;
using HealthNet.DTOs.PateintDto;
using HealthNet.DTOs.Pages;
using HealthNetDb.Entities;
using HealthNetDb.Data;
using Microsoft.EntityFrameworkCore;
using HealthNet.Repository.PatientRepository;



namespace HealthNet.Services.PatientServices;

public class PatientManagementService : IPatientManagementService
{
        private readonly IPatientManagementRepository _patientRepository;
     private readonly HealthNetContext _context;

    public PatientManagementService(IPatientManagementRepository patientRepository,HealthNetContext context)
    {
        _patientRepository = patientRepository;
        _context=context;
    }

    public async Task<PagedResponseDto<Patient>> SearchPatientsAsync(PatientSearchDto searchDto,int userId)
    {
        
  var result = await _patientRepository.SearchPatientsAsync(searchDto);

        // 2️⃣ Get ActionId for READ (ActionId = 3)
        var actionId = await _context
            .Set<HealthNetDb.Entities.Action>()   // avoids System.Action conflict
            .Where(a => a.ActionName == "Read")
            .Select(a => a.ActionId)
            .FirstAsync();

        // 3️⃣ Create Audit Log
        var auditLog = new AuditLog
        {
            UserId = userId,        // 0 = System / Anonymous
            ActionId = actionId,    // READ
            Resource = "PatienRecord",
            Timestamp = DateTime.UtcNow
        };

        // 4️⃣ Save Audit Log
        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();

        return result;

        //return await _patientRepository.SearchPatientsAsync(searchDto);
    }
}
