using System;
using System.Security.Claims;
using HealthNet.DTOs.OutbreakMonitoringDTO;
using HealthNet.Repository.OutbreakMonitoringRepository;
using HealthNet.Utility;
using HealthNetDb.Data;
using HealthNetDb.Entities;

namespace HealthNet.Services.OutbreakMonitoringServices;

public class OutbreakMonitoringServices : IOutbreakMonitoringServices
{
    private readonly IOutBreakMonitoringRepository _repository;
    private readonly LocationHelper _locationHelper;
    public OutbreakMonitoringServices(IOutBreakMonitoringRepository repository, LocationHelper locationHelper)
    {
        _repository = repository;
        _locationHelper = locationHelper;
    } 

    // Add Outbreak Service
    /// <summary>
    /// The outbreakwill be validated and sent to the repository
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="request"></param>
    /// <returns>
    /// If Outbreak Creation is Succesfull  returns DTO which consist of Message,and OutbreakId else ErrorMessage
    /// </returns>
    /// <exception cref="HealthNetException"></exception>
    public async Task<CreateOutbreakResponseDto> AddOutbreakService(int userId, CreateOutbreakRequestDto request)
    {
        try
        {
            // Validate the Disease field
            if (string.IsNullOrWhiteSpace(request.Disease))
            {
                return new CreateOutbreakResponseDto
                {
                    Success = false,
                    Message = "Disease field Cannot be Empty."
                };
            }
            // Check for Special Chars in Disease
            if (StringHelper.ContainsSpecialCharacters(request.Disease))
            {
                return new CreateOutbreakResponseDto
                {
                    Success = false,
                    Message = "Special Characters are not allowed in Disease field."
                };
            }
            // Validate the Severity field
            if (string.IsNullOrWhiteSpace(request.Severity))
            {
                return new CreateOutbreakResponseDto
                {
                    Success = false,
                    Message = "Severity field Cannot be Empty."
                };
            }
            // Validate the Location field
            if (string.IsNullOrWhiteSpace(request.Location))
            {
                return new CreateOutbreakResponseDto
                {
                    Success = false,
                    Message = "Location cannot be Empty."
                };
            }
            if (request.Location.Length < 3)
            {
                return new CreateOutbreakResponseDto
                {
                    Success = false,
                    Message = "Location must be of atleast 3 Characters length."
                };
            }
            
            bool isValidLocation = await _locationHelper.LocationValidatorAsync(request.Location);
            if (!isValidLocation)
            {
                return new CreateOutbreakResponseDto
                {
                    Success = false,
                    Message = "Please Enter a valid Location."
                };
            }

            var presentDateTime = DateTime.UtcNow;
            // Validate Start Date
            if (request.StartDate > presentDateTime)
            {
                return new CreateOutbreakResponseDto
                {
                    Success = false,
                    Message = "Start Date Cannot be in Future."
                };
            }
            // Validate End Date
            if (request.EndDate > presentDateTime)
            {
                return new CreateOutbreakResponseDto
                {
                    Success = false,
                    Message = "End Date Cannot be in Future."
                };
            }
            // Check Whether the Start Date is Before the End Date or not.
            if (request.StartDate >= request.EndDate)
            {
                return new CreateOutbreakResponseDto
                {
                    Success = false,
                    Message = "Start date must be earlier than the end date."
                };
            }
            // Check for Duplicate Outbreak
            if (await _repository.DuplicateOutbreakExitsAsync(request))
            {
                return new CreateOutbreakResponseDto
                {
                    Success = false,
                    Message = $"{request.Disease} in {request.Location} is already in Active State."
                };
            }

            // Return Outbreak with Success message for valid Data.
            int outbreakid = await _repository.AddOutbreakAsync(request);

            // Record it in Audit Log if Every thing works well
            await _repository.AddAuditLogAsync(userId, "Outbreak");

            // Return the result 
            return new CreateOutbreakResponseDto
            {
                Success = true,
                Message = "Outbreak Created Successfully.",
                OutbreakId = outbreakid,
            };
        }
        catch(Exception ex)
        {
            throw new HealthNetException(ex.Message);
        }
    }
}
