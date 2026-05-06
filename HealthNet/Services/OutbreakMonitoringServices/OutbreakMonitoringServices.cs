using System;
using System.Security.Claims;
using System.Text.Json;
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
            var allowedSeverities = new[] { "high", "low", "medium" };
            if (string.IsNullOrWhiteSpace(request.Severity))
            {
                return new CreateOutbreakResponseDto
                {
                    Success = false,
                    Message = "Severity field Cannot be Empty."
                };
            }
            if (!allowedSeverities.Contains(request.Severity.ToLower()))
            {
                return new CreateOutbreakResponseDto
                {
                    Success = false,
                    Message = "Severity field must be one of these values High, Medium, Low."
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

            // Check for Special Chars in Loction
            if (StringHelper.ContainsSpecialCharacters(request.Location))
            {
                return new CreateOutbreakResponseDto
                {
                    Success = false,
                    Message = "Special Characters are not allowed in Location field."
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
            // If end Date is Less than the present Date then make Status as inactive
            if (presentDateTime <= request.EndDate)
            {
                request.Status = false;
            }

            // Check for Duplicate Outbreak
            if (await _repository.DuplicateOutbreakExitsAsync(request))
            {
                return new CreateOutbreakResponseDto
                {
                    Success = false,
                    Message = $"{request.Disease} in {request.Location} is already Exists in Database we cannot update here."
                };
            }

            // Return Outbreak with Success message for valid Data.
            int outbreakid = await _repository.AddOutbreakAsync(request);

            // Record it in Audit Log if Every thing works well
            await _repository.AddAuditLogAsync(userId, "Create", "Outbreak");

            // Return the result 
            return new CreateOutbreakResponseDto
            {
                Success = true,
                Message = "Outbreak Created Successfully.",
                OutbreakId = outbreakid,
            };
        }
        catch (Exception ex)
        {
            throw new HealthNetException(ex.Message);
        }
    }
    //GetOutbreakById
    public async Task<GetOutbreakResponseDto?> GetOutbreakByIdService(int outbreakId)
    {
        var outbreak = await _repository.GetOutbreakByIdAsync(outbreakId);
        if (outbreak == null)
            return null;
        return new GetOutbreakResponseDto
        {
            OutbreakId = outbreak.OutbreakId,
            Disease = outbreak.Disease,
            Location = outbreak.Location,
            StartDate = outbreak.StartDate,
            EndDate = outbreak.EndDate,
            Severity = outbreak.Severity,
            Status = outbreak.Status
        };
    }

    //updating outreak
    public async Task<UpdateOutbreakResponseDto> UpdateOutbreakService(int userId, int outbreakId, UpdateOutbreakRequestDto request)
    {
        // Severity empty check
        if (string.IsNullOrWhiteSpace(request.Severity))
        {
            return new UpdateOutbreakResponseDto
            {
                Success = false,
                Message = "Severity cannot be empty"
            };
        }
        // Allowed severity values
        var allowedSeverity = new[] { "Low", "Medium", "High" };
        if (!allowedSeverity.Contains(request.Severity, StringComparer.OrdinalIgnoreCase))
        {
            return new UpdateOutbreakResponseDto
            {
                Success = false,
                Message = "Severity must be Low, Medium, or High"
            };
        }
        // End date validation
        if (request.EndDate.Date > DateTime.UtcNow.Date)
        {
            return new UpdateOutbreakResponseDto
            {
                Success = false,
                Message = "End date cannot be a future date"
            };
        }
        //FETCH EXISTING OUTBREAK FIRST
        var existingOutbreak = await _repository.GetOutbreakByIdAsync(outbreakId);
        if (existingOutbreak == null)
        {
            return new UpdateOutbreakResponseDto
            {
                Success = false,
                Message = "Outbreak not found"
            };
        }
        //EndDate vs StartDate validation
        if (request.EndDate < existingOutbreak.StartDate)
        {
            return new UpdateOutbreakResponseDto
            {
                Success = false,
                Message = "End date cannot be earlier than the start date"
            };
        }
        //Auto-close if EndDate is in the past
        if (request.EndDate.Date < DateTime.UtcNow.Date)
        {
            request.Status = false;
        }
        var result = await _repository.UpdateOutbreakAsync(outbreakId, request);
        if (result == UpdateOutbreakResult.NotFound)
        {
            return new UpdateOutbreakResponseDto
            {
                Success = false,
                Message = "Outbreak not found"
            };
        }
        //status validation
        if (result == UpdateOutbreakResult.Closed)
        {
            return new UpdateOutbreakResponseDto
            {
                Success = false,
                Message = "This outbreak is already closed and cannot be updated."
            };
        }
        if (result == UpdateOutbreakResult.NoChanges)
        {
            return new UpdateOutbreakResponseDto
            {
                Success = false,
                Message = "No changes detected. Update request has no effect."
            };
        }
        await _repository.AddAuditLogAsync(userId, "Update", "Outbreak");
        return new UpdateOutbreakResponseDto
        {
            Success = true,
            Message = $"Outbreak updated successfully for Id {outbreakId}"
        };
    }
    public async Task<AddEpidemiologyResponseDto> AddEpidemiologyService(int userId, int outbreakId, AddEpidemiologyRequestDto request)
    {
        //  MetricsJSON validatio
        if (string.IsNullOrWhiteSpace(request.MetricsJSON))
        {
            return new AddEpidemiologyResponseDto
            {
                Success = false,
                Message = "MetricsJSON cannot be empty"
            };
        }
        // Date must NOT be future
        if (request.Date.Date > DateTime.UtcNow.Date)
        {
            return new AddEpidemiologyResponseDto
            {
                Success = false,
                Message = "Epidemiology date cannot be a future date"
            };
        }
        //  Outbreak existence check

        var outbreak = await _repository.GetOutbreakByIdAsync(outbreakId);

        if (outbreak == null)
        {
            return new AddEpidemiologyResponseDto
            {
                Success = false,
                Message = "Outbreak not found"
            };
        }
        //  Metric date must not be before outbreak start date
        if (request.Date.Date < outbreak.StartDate.Date)
        {
            return new AddEpidemiologyResponseDto
            {
                Success = false,
                Message = "Epidemiology date cannot be before outbreak start date"
            };
        }
        // If outbreak is closed, metric date must not be after outbreak end date
        if (!outbreak.Status && request.Date.Date > outbreak.EndDate.Date)
        {
            return new AddEpidemiologyResponseDto
            {
                Success = false,
                Message = "Epidemiology date cannot be after outbreak end date for a closed outbreak"
            };
        }
        // Parse MetricsJSON
        JsonDocument doc;
        try
        {
            doc = JsonDocument.Parse(request.MetricsJSON);
        }
        catch
        {
            return new AddEpidemiologyResponseDto
            {
                Success = false,
                Message = "MetricsJSON must be valid JSON"
            };
        }

        var root = doc.RootElement;
        //  Enforce required metrics only
        if (!root.TryGetProperty("cases", out var casesProp) ||
            !root.TryGetProperty("recoveries", out var recProp) ||
            !root.TryGetProperty("RtNow", out var rtProp))
        {
            return new AddEpidemiologyResponseDto
            {
                Success = false,
                Message = "MetricsJSON must contain cases, recoveries, and RtNow"
            };
        }
        int cases = casesProp.GetInt32();
        int recoveries = recProp.GetInt32();
        double rtNow = rtProp.GetDouble();
        // Metrics value validation
        if (cases < 0 || recoveries < 0 || rtNow < 0)
        {
            return new AddEpidemiologyResponseDto
            {
                Success = false,
                Message = "Metric values cannot be negative"
            };
        }

        if (recoveries > cases)
        {
            return new AddEpidemiologyResponseDto
            {
                Success = false,
                Message = "Recoveries cannot be greater than cases"
            };
        }
        // Create Epidemiology record
        var epi = new Epidemiology
        {
            OutbreakId = outbreakId,
            MetricsJSON = request.MetricsJSON,
            Date = request.Date,
            Status = request.Status
        };

        int epiId = await _repository.AddEpidemiologyAsync(epi);

        // ADD TRIGGER
        if (OutbreakAlertUtility.IsThresholdBreached(rtNow))
        {
            Console.WriteLine($"🚨 OutbreakAlertRaised | OutbreakId={outbreakId} | Rt={rtNow} | Time={DateTime.UtcNow}");
        }

        await _repository.AddAuditLogAsync(userId, "Create", "Epidemiology");

        return new AddEpidemiologyResponseDto
        {
            Success = true,
            Message = "Epidemiology metrics stored successfully",
            EpiId = epiId
        };
    }
    public async Task<List<GetActiveOutbreaksResponseDto>> GetAllActiveOutbreaksService()
    {
        var outbreaks = await _repository.GetAllActiveOutbreaksAsync();

        return outbreaks.Select(o => new GetActiveOutbreaksResponseDto
        {
            OutbreakId = o.OutbreakId,
            Disease = o.Disease,
            Location = o.Location,
            StartDate = o.StartDate,
            EndDate = o.EndDate,
            Severity = o.Severity,
            Status = o.Status
        }).ToList();
    }
}