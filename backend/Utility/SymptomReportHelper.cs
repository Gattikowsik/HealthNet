using System;
using System.Text.Json;
namespace HealthNet.Utility;
public class SymptomReportHelper
{
    public static bool IsValidJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return false;
            
        try
        {
            JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }

}
