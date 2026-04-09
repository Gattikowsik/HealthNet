using HealthNetDb.Entities;

namespace HealthNet.Repository.LabTestRepo;

public interface ILaboratoryTestingRepository
{
    Task<bool> PatientExistsAsync(int patientId);
    Task<LabTest> CreateLabTestAsync(LabTest labTest);
    Task<bool> TechnicianExistsAsync(int technicianId);
    Task<bool> DuplicateTestExistsAsync(int patientId, string type);
}