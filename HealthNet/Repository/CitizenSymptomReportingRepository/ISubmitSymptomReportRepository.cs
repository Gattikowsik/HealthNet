using System;
using HealthNetDb.Entities;
namespace HealthNet.Repository;
public interface ISubmitSymptomReportRepository
{
    Task<SymptomReport> AddAsync(SymptomReport report);
}
