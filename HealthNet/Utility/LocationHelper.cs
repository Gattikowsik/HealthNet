using System.Text.Json;
using HealthNet.DTOs.OutbreakMonitoringDTO;

namespace HealthNet.Utility;

public class LocationHelper
{
    private readonly HttpClient _httpClient;

    public LocationHelper(HttpClient httpClient)
    {
        _httpClient = httpClient;

        if (!_httpClient.DefaultRequestHeaders.UserAgent.Any())
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "HealthNetApp (patelpranay577@gmail.com)"
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