using System;
using HealthNet.DTOs.LabTestDTO;

namespace HealthNet.Services.LabTestServices;

public interface ILabTestService
{
    Task<LabTestResponse> CreateLabTestAsync(LabTestRequest request);
}
