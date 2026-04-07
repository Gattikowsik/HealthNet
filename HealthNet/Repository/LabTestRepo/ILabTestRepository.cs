using HealthNetDb.Entities;

namespace HealthNet.Repository.LabTestRepo;

public interface ILabTestRepository
{
    Task<bool> PatientExistsAsync(int patientId);
    Task<LabTest> CreateLabTestAsync(LabTest labTest);
}