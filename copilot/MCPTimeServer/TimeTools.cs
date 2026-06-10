using ModelContextProtocol.Server;

using NodaTime;

namespace MCPTimeServer;

/// <summary>
/// MCP Server tools for querying current time in different locations.
/// </summary>
[McpServerToolType]
public static class TimeTools
{
    // Mapping of city names to IANA time zone identifiers
    private static readonly Dictionary<string, string> CityTimezones = new()
    {
        ["London"] = "Europe/London",
        ["New York"] = "America/New_York",
        ["Tokyo"] = "Asia/Tokyo",
        ["Sydney"] = "Australia/Sydney",
        ["Berlin"] = "Europe/Berlin",
        ["Paris"] = "Europe/Paris",
        ["Los Angeles"] = "America/Los_Angeles",
        ["Chicago"] = "America/Chicago",
        ["Hong Kong"] = "Asia/Hong_Kong",
        ["Singapore"] = "Asia/Singapore",
        ["Dubai"] = "Asia/Dubai",
        ["Mumbai"] = "Asia/Kolkata",
        ["Beijing"] = "Asia/Shanghai",
        ["São Paulo"] = "America/Sao_Paulo",
        ["Mexico City"] = "America/Mexico_City",
        ["Moscow"] = "Europe/Moscow",
        ["Toronto"] = "America/Toronto",
        ["Seoul"] = "Asia/Seoul",
        ["Shanghai"] = "Asia/Shanghai",
        ["Amsterdam"] = "Europe/Amsterdam",
        ["Vienna"] = "Europe/Vienna",
        ["Lingen"] = "Europe/Berlin"  // Lingen, Germany uses Berlin timezone
    };

    /// <summary>
    /// Gets the current local time in the system's default timezone.
    /// </summary>
    /// <returns>Current local time with timezone information</returns>
    [McpServerTool]
    [System.ComponentModel.Description("Get the current local time in the system's timezone")]
    public static string GetLocalTime()
    {
        DateTimeZone systemZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
        Instant now = SystemClock.Instance.GetCurrentInstant();
        ZonedDateTime localTime = now.InZone(systemZone);

        return $"""
            Current Local Time:
            Time: {localTime:F}
            Timezone: {systemZone.Id}
            UTC Offset: {localTime.Offset}
            """;
    }

    /// <summary>
    /// Gets the current time in a specific city or timezone.
    /// </summary>
    /// <param name="location">City name (e.g., "London", "New York") or IANA timezone identifier (e.g., "America/New_York")</param>
    /// <returns>Current time in the specified location</returns>
    [McpServerTool]
    [System.ComponentModel.Description("Get the current time in a specific city or timezone. Supports major cities like London, New York, Tokyo, Sydney, Berlin, and IANA timezone identifiers.")]
    public static string GetCityTime(
        [System.ComponentModel.Description("City name (e.g., 'London', 'New York', 'Tokyo') or IANA timezone identifier (e.g., 'America/New_York')")]
        string location)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            return "Error: Location cannot be empty. Please provide a city name or timezone identifier.";
        }

        // Try to get the timezone - first check if it's a known city
        DateTimeZone? timeZone = null;
        string timezoneId;

        if (CityTimezones.TryGetValue(location, out string? mappedTimezone))
        {
            timezoneId = mappedTimezone;
            timeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(timezoneId);
        }
        else
        {
            // Try to use it as a direct timezone identifier
            timezoneId = location;
            timeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(location);
        }

        if (timeZone == null)
        {
            List<string> supportedCities = [.. CityTimezones.Keys.Order()];
            return $"""
                Error: Unknown location '{location}'.
                
                Supported cities:
                {string.Join(", ", supportedCities)}
                
                You can also use IANA timezone identifiers like:
                - America/New_York
                - Europe/London
                - Asia/Tokyo
                
                For a complete list of timezones, visit: https://en.wikipedia.org/wiki/List_of_tz_database_time_zones
                """;
        }

        Instant now = SystemClock.Instance.GetCurrentInstant();
        ZonedDateTime cityTime = now.InZone(timeZone);

        return $"""
            Current Time in {location}:
            Time: {cityTime:F}
            Timezone: {timezoneId}
            UTC Offset: {cityTime.Offset}
            Date: {cityTime.Date}
            Day of Week: {cityTime.DayOfWeek}
            """;
    }

    /// <summary>
    /// Lists all supported major cities.
    /// </summary>
    /// <returns>List of all supported city names</returns>
    [McpServerTool]
    [System.ComponentModel.Description("Get a list of all supported major cities for time queries")]
    public static string ListSupportedCities()
    {
        List<string> cities = [.. CityTimezones.Keys.Order()];
        
        return $"""
            Supported Major Cities ({cities.Count} total):
            
            {string.Join("\n", cities.Select((city, index) => $"{index + 1,2}. {city,-20} ({CityTimezones[city]})"))}
            
            You can also use any IANA timezone identifier directly.
            For a complete list, visit: https://en.wikipedia.org/wiki/List_of_tz_database_time_zones
            """;
    }

    /// <summary>
    /// Compares time between two locations.
    /// </summary>
    /// <param name="location1">First city or timezone</param>
    /// <param name="location2">Second city or timezone</param>
    /// <returns>Time comparison between the two locations</returns>
    [McpServerTool]
    [System.ComponentModel.Description("Compare current time between two cities or timezones")]
    public static string CompareTime(
        [System.ComponentModel.Description("First city name or IANA timezone identifier")]
        string location1,
        [System.ComponentModel.Description("Second city name or IANA timezone identifier")]
        string location2)
    {
        if (string.IsNullOrWhiteSpace(location1) || string.IsNullOrWhiteSpace(location2))
        {
            return "Error: Both locations must be provided.";
        }

        // Get timezone for location 1
        DateTimeZone? tz1 = GetTimezoneForLocation(location1);
        if (tz1 == null)
        {
            return $"Error: Unknown location '{location1}'. Use ListSupportedCities to see available cities.";
        }

        // Get timezone for location 2
        DateTimeZone? tz2 = GetTimezoneForLocation(location2);
        if (tz2 == null)
        {
            return $"Error: Unknown location '{location2}'. Use ListSupportedCities to see available cities.";
        }

        Instant now = SystemClock.Instance.GetCurrentInstant();
        ZonedDateTime time1 = now.InZone(tz1);
        ZonedDateTime time2 = now.InZone(tz2);

        // Calculate timezone offset difference
        // This correctly handles DST because we get the current offset for each zone at the same instant
        Offset offset1 = time1.Offset;
        Offset offset2 = time2.Offset;
        int offsetDiffSeconds = offset2.Seconds - offset1.Seconds;
        double offsetDiffHours = offsetDiffSeconds / 3600.0;

        return $"""
            Time Comparison:
            
            {location1}:
            Time: {time1:F}
            Timezone: {tz1.Id}
            UTC Offset: {time1.Offset}
            
            {location2}:
            Time: {time2:F}
            Timezone: {tz2.Id}
            UTC Offset: {time2.Offset}
            
            Time Difference: {Math.Abs(offsetDiffHours):F1} hour(s) ({(offsetDiffHours > 0 ? $"{location2} is ahead" : offsetDiffHours < 0 ? $"{location1} is ahead" : "Same time")})
            """;
    }

    private static DateTimeZone? GetTimezoneForLocation(string location)
    {
        if (CityTimezones.TryGetValue(location, out string? mappedTimezone))
        {
            return DateTimeZoneProviders.Tzdb.GetZoneOrNull(mappedTimezone);
        }
        
        return DateTimeZoneProviders.Tzdb.GetZoneOrNull(location);
    }
}
