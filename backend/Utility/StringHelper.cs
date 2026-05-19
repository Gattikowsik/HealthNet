using System;
using System.Text.RegularExpressions;

namespace HealthNet.Utility;

public class StringHelper
{
    public static bool ContainsSpecialCharacters(string input)
    {
        return Regex.IsMatch(input, @"[^a-zA-Z0-9 ]");
    }
}
