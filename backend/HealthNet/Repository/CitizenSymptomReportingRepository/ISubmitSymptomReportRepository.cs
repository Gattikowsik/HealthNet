using System;
using HealthNetDb.Entities;
namespace HealthNet.Repository;
public interface ISubmitSymptomReportRepository
{
    Task<SymptomReport> AddAsync(SymptomReport report);
    Task<(List<SymptomReport> Reports, int TotalCount)> GetMineAsync(int citizenId, int pageNumber, int pageSize);
    Task<(List<SymptomReport> Reports, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
}
