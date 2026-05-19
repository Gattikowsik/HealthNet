using System;
using HealthNet.DTOs.LabTestDTO;

namespace HealthNet.Services.LabTestServices;

public interface ILaboratoryTestingService
{
    Task<LaboratoryTestingResponse> CreateLaboratoryTestAsync(LaboratoryTestingRequest request, int userId);
    Task<IEnumerable<LaboratoryTestingResponse>> GetLabTestsAsync(LaboratoryTestingFilterRequest filter, int userId);
    Task<LaboratoryTestingResponse> UpdateLabTestAsync(int testId, LaboratoryTestingUpdateRequest request, int userId, string userRole);
}