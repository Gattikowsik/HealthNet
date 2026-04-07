using System;
using HealthNetDb.Entities;
namespace HealthNet.Repository;
public interface ISymptomReportRepository
{
    Task<SymptomReport> AddAsync(SymptomReport report);
}
