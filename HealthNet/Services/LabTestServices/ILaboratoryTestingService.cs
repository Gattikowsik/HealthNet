using System;
using HealthNet.DTOs.LabTestDTO;

namespace HealthNet.Services.LabTestServices;

public interface ILaboratoryTestingService
{
    Task<LaboratoryTestingResponse> CreateLaboratoryTestAsync(LaboratoryTestingRequest request, int userId);
    Task<IEnumerable<LaboratoryTestingResponse>> GetLabTestsAsync(LaboratoryTestingFilterRequest filter, int userId);
}