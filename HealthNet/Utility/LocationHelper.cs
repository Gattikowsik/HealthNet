using System.Text.Json;
using HealthNet.DTOs.OutbreakMonitoringDTO;

namespace HealthNet.Utility;

public class LocationHelper
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    public LocationHelper(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;

        if (!_httpClient.DefaultRequestHeaders.UserAgent.Any())
        {
            // string cred = "HealthNetApp "+_configuration["LogDetails:Email"];
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "HealthNetApp/1.0"
            );

            _httpClient.DefaultRequestHeaders.Add(
                    "Contact-Email",
                    configuration["LogDetails:Email"]
                );
        }
    }

    public async Task<bool> LocationValidatorAsync(
        string location,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(location))
            return false;

        var url =
            "https://nominatim.openstreetmap.org/search" +
            $"?q={Uri.EscapeDataString(location)}" +
            "&format=json" +
            "&limit=1" +
            "&countrycodes=in";

        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return false;

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            var results = JsonSerializer.Deserialize<List<LocationResponseDto>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return results is { Count: > 0 };
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}