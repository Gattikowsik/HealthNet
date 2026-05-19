using HealthNetDb.Entities;
using HealthNet.DTOs.LabTestDTO;

namespace HealthNet.Repository.LabTestRepo;

public interface ILaboratoryTestingRepository
{
    Task<bool> PatientExistsAsync(int patientId);
    Task<LabTest> CreateLabTestAsync(LabTest labTest);
    Task<bool> TechnicianExistsAsync(int technicianId);
    Task<bool> DuplicateTestExistsAsync(int patientId, string type);
    Task<IEnumerable<LabTest>> GetLabTestsAsync(LaboratoryTestingFilterRequest filter, bool? statusBool);
    Task<LabTest?> GetLabTestByIdAsync(int testId);
    Task<LabTest> UpdateLabTestAsync(LabTest labTest);
}