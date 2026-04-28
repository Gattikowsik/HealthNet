using System;
using HealthNetDb.Entities;

namespace HealthNet.Repository.MedicalRepository;

public interface IMedicalRecordRepository
{
    Task<MedicalRecord> AddAsync(MedicalRecord record);
}
