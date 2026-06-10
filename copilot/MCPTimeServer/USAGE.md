# MCP Time Server - Usage Examples

This document provides practical examples of using the MCP Time Server with AI assistants and MCP clients.

## Tool Examples

### 1. Get Local Time

**Request:**
```json
{
  "tool": "GetLocalTime",
  "arguments": {}
}
```

**Response:**
```
Current Local Time:
Time: Tuesday, 17 December 2024 5:15:30 AM UTC
Timezone: Etc/UTC
UTC Offset: +00
```

### 2. Get City Time

**Request:**
```json
{
  "tool": "GetCityTime",
  "arguments": {
    "location": "Tokyo"
  }
}
```

**Response:**
```
Current Time in Tokyo:
Time: Tuesday, 17 December 2024 2:15:30 PM JST
Timezone: Asia/Tokyo
UTC Offset: +09
Date: Tuesday, 17 December 2024
Day of Week: Tuesday
```

### 3. List Supported Cities

**Request:**
```json
{
  "tool": "ListSupportedCities",
  "arguments": {}
}
```

**Response:**
```
Supported Major Cities (20 total):

 1. Amsterdam          (Europe/Amsterdam)
 2. Beijing            (Asia/Shanghai)
 3. Berlin             (Europe/Berlin)
 4. Chicago            (America/Chicago)
 5. Dubai              (Asia/Dubai)
 6. Hong Kong          (Asia/Hong_Kong)
 7. London             (Europe/London)
 8. Los Angeles        (America/Los_Angeles)
 9. Mexico City        (America/Mexico_City)
10. Moscow             (Europe/Moscow)
11. Mumbai             (Asia/Kolkata)
12. New York           (America/New_York)
13. Paris              (Europe/Paris)
14. São Paulo          (America/Sao_Paulo)
15. Seoul              (Asia/Seoul)
16. Shanghai           (Asia/Shanghai)
17. Singapore          (Asia/Singapore)
18. Sydney             (Australia/Sydney)
19. Tokyo              (Asia/Tokyo)
20. Toronto            (America/Toronto)

You can also use any IANA timezone identifier directly.
```

### 4. Compare Time Between Cities

**Request:**
```json
{
  "tool": "CompareTime",
  "arguments": {
    "location1": "New York",
    "location2": "Tokyo"
  }
}
```

**Response:**
```
Time Comparison:

New York:
Time: Tuesday, 17 December 2024 12:15:30 AM EST
Timezone: America/New_York
UTC Offset: -05

Tokyo:
Time: Tuesday, 17 December 2024 2:15:30 PM JST
Timezone: Asia/Tokyo
UTC Offset: +09

Time Difference: 14.0 hour(s) (Tokyo is ahead)
```

## Natural Language Examples

When integrated with AI assistants, you can use natural language:

### Example 1: Basic Time Query
**User:** "What time is it in London?"

**AI Assistant:** Uses the `GetCityTime` tool with `location: "London"`

**Result:** Current time in London with full details

### Example 2: Time Comparison
**User:** "Compare the time between Sydney and Berlin"

**AI Assistant:** Uses the `CompareTime` tool with `location1: "Sydney"` and `location2: "Berlin"`

**Result:** Time comparison showing both cities and the time difference

### Example 3: Multiple Queries
**User:** "What time is it in New York, Tokyo, and Paris?"

**AI Assistant:** Calls `GetCityTime` three times in parallel

**Result:** Current times for all three cities

### Example 4: Meeting Planning
**User:** "If it's 9 AM in Los Angeles, what time is it in Mumbai?"

**AI Assistant:** Uses `CompareTime` to determine the time difference, then calculates

**Result:** Time in Mumbai when it's 9 AM in Los Angeles (10:30 PM same day)

## Integration with GitHub Copilot

In Visual Studio with GitHub Copilot, you can ask questions like:

```
"What cities does this MCP server support?"
"Add support for Cairo timezone"
"How do I add a new tool for sunrise times?"
"Refactor the city dictionary to use a more maintainable structure"
```

## Integration with Claude Desktop

To use with Claude Desktop:

1. Add to your `claude_desktop_config.json`:
```json
{
  "mcpServers": {
    "time-server": {
      "command": "dotnet",
      "args": ["run", "--project", "/path/to/MCPTimeServer/MCPTimeServer.csproj"]
    }
  }
}
```

2. Restart Claude Desktop

3. Ask Claude: "What time is it in Tokyo?"

## Error Handling Examples

### Unknown City

**Request:**
```json
{
  "tool": "GetCityTime",
  "arguments": {
    "location": "Atlantis"
  }
}
```

**Response:**
```
Error: Unknown location 'Atlantis'.

Supported cities:
Amsterdam, Beijing, Berlin, Chicago, Dubai, Hong Kong, London, Los Angeles, Mexico City, Moscow, Mumbai, New York, Paris, São Paulo, Seoul, Shanghai, Singapore, Sydney, Tokyo, Toronto

You can also use IANA timezone identifiers like:
- America/New_York
- Europe/London
- Asia/Tokyo

For a complete list of timezones, visit: https://en.wikipedia.org/wiki/List_of_tz_database_time_zones
```

### Using IANA Timezone Directly

**Request:**
```json
{
  "tool": "GetCityTime",
  "arguments": {
    "location": "Pacific/Auckland"
  }
}
```

**Response:**
```
Current Time in Pacific/Auckland:
Time: Tuesday, 17 December 2024 6:15:30 PM NZDT
Timezone: Pacific/Auckland
UTC Offset: +13
Date: Tuesday, 17 December 2024
Day of Week: Tuesday
```

## Best Practices

1. **Always validate timezones**: Use IANA timezone identifiers for accuracy
2. **Handle errors gracefully**: The server provides helpful error messages
3. **Consider DST changes**: NodaTime handles daylight saving time automatically
4. **Use CompareTime for scheduling**: When planning meetings across timezones
5. **Cache city list**: Call `ListSupportedCities` once to get all available cities

## Performance Notes

- All operations are lightweight and fast
- No external API calls required
- Uses system time and NodaTime database
- Suitable for high-frequency queries
- Thread-safe operations

## Extending the Server

To add new cities, edit `TimeTools.cs`:

```csharp
private static readonly Dictionary<string, string> CityTimezones = new()
{
    // ... existing cities ...
    ["Cairo"] = "Africa/Cairo",
    ["Istanbul"] = "Europe/Istanbul",
    ["Buenos Aires"] = "America/Argentina/Buenos_Aires"
};
```

Rebuild and the new cities are immediately available!
