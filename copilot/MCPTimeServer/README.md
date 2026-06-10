# MCP Time Server - Model Context Protocol Server for Time Queries

A **Model Context Protocol (MCP)** server built with .NET 10 and the MCP C# SDK  that provides intelligent time query capabilities. This server exposes tools for querying current time across the world, supporting both local time queries and major cities worldwide.

## 🌍 Overview

The MCP Time Server allows AI assistants like GitHub Copilot, Claude, and other MCP clients to query current time information through a standardized protocol. It provides:

- **Local Time Queries**: Get the current time in your system's timezone
- **City Time Queries**: Get current time in major world cities
- **Timezone Support**: Support for IANA timezone identifiers
- **Time Comparison**: Compare time between two locations
- **City Discovery**: List all supported major cities

## 🚀 Features

### Supported Cities

The server includes built-in support for 22 major world cities:

- **Americas**: New York, Los Angeles, Chicago, Toronto, São Paulo, Mexico City
- **Europe**: London, Paris, Berlin, Amsterdam, Moscow, Vienna, Lingen (Germany)
- **Asia**: Tokyo, Beijing, Shanghai, Hong Kong, Singapore, Seoul, Dubai, Mumbai
- **Oceania**: Sydney

### Available Tools

1. **GetLocalTime** - Get current time in the system's timezone
2. **GetCityTime** - Get current time in a specific city or timezone
3. **ListSupportedCities** - View all supported cities with their IANA timezone identifiers
4. **CompareTime** - Compare current time between two locations

## 📋 Prerequisites

- **.NET 10 SDK** (version 10.0.101 or later)
- **Visual Studio 2026** or **VS Code** with C# extension
- **NuGet Package Manager** for installing preview packages

## 🔧 Setup Instructions

### 1. Install .NET 10 SDK

Download and install the .NET 10 SDK from:
- **Official Download**: https://dotnet.microsoft.com/download/dotnet/10.0

Verify installation:
```bash
dotnet --version
# Should display 10.0.101 or later
```

### 2. Clone or Navigate to the Repository

```bash
cd /path/to/Dotnet10Samples/src/MCPTimeServer
```

### 3. Restore NuGet Packages

The project uses preview packages. Restore them with:

```bash
dotnet restore
```

**Key Dependencies:**
- `ModelContextProtocol` (v0.5.0-preview.1) - MCP SDK for .NET
- `NodaTime` (v3.2.0) - Robust timezone handling library
- `Microsoft.Extensions.Hosting` (v10.0.0) - Hosting infrastructure

### 4. Build the Project

```bash
dotnet build
```

Expected output:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### 5. Run the Server

```bash
dotnet run
```

The server will start and listen for MCP protocol messages via standard input/output (StdIO transport).

## 🎯 Using Visual Studio

### Opening the Project

1. **Open Visual Studio 2024**
2. Select **File → Open → Project/Solution**
3. Navigate to `Dotnet10Samples/src/MCPTimeServer/`
4. Select `MCPTimeServer.csproj`

### Adding Preview Packages in Visual Studio

The project already includes the necessary packages, but to add MCP SDK to other projects:

1. Right-click the project in **Solution Explorer**
2. Select **Manage NuGet Packages**
3. Check **Include prerelease**
4. Search for `ModelContextProtocol`
5. Install `ModelContextProtocol` (preview version)

### Building and Running

#### Build:
- **Menu**: Build → Build Solution
- **Keyboard**: `Ctrl+Shift+B`

#### Run:
- **Menu**: Debug → Start Without Debugging
- **Keyboard**: `Ctrl+F5`

#### Debug:
- **Menu**: Debug → Start Debugging
- **Keyboard**: `F5`

## 🤖 GitHub Copilot Integration in Visual Studio

### Enabling GitHub Copilot

1. **Install GitHub Copilot Extension**
   - Go to **Extensions → Manage Extensions**
   - Search for "GitHub Copilot"
   - Install and restart Visual Studio

2. **Sign In**
   - Go to **Tools → Options → GitHub → Copilot**
   - Sign in with your GitHub account
   - Ensure your account has Copilot access

### Using Copilot with This Project

#### Code Completion
- Start typing and Copilot will suggest completions
- Press `Tab` to accept suggestions
- Press `Esc` to dismiss

#### Adding New Cities
```csharp
// Start typing and Copilot will suggest the pattern:
["Cairo"] = "Africa/Cairo",
["Istanbul"] = "Europe/Istanbul",
```

#### Creating New Tools
```csharp
// Type a comment and Copilot will generate the method:
/// <summary>
/// Gets the sunrise and sunset times for a city
/// </summary>
[McpServerTool]
[System.ComponentModel.Description("Get sunrise and sunset times for a city")]
public static string GetSunriseSunset(
    [System.ComponentModel.Description("City name")]
    string city)
{
    // Copilot will suggest implementation
}
```

#### Refactoring with Copilot
1. Select code you want to refactor
2. Press `Ctrl+I` to open Copilot Chat
3. Ask "Refactor this using modern C# features"
4. Review and accept suggestions

### Copilot Chat Commands

Open Copilot Chat with `Ctrl+/` and try:

```
/explain - Explain how the TimeTools class works
/fix - Fix any issues in the selected code
/optimize - Suggest performance improvements
/tests - Generate unit tests for a method
```

### Advanced Copilot Features

#### Inline Chat (`Ctrl+I`)
- Select code and press `Ctrl+I`
- Ask questions about the code
- Request modifications in natural language

#### Planning Mode
For complex features:
1. Open Copilot Chat
2. Describe the feature: "Add support for recurring timezone reminders"
3. Copilot will break down the task into steps
4. Follow the plan to implement

## 🔌 Configuring MCP Time Server with Visual Studio Copilot

To make this MCP server available as a tool in Visual Studio Copilot (Agent Mode), you need to configure it in a `.mcp.json` file.

### Prerequisites

- **Visual Studio 2022 Version 17.14 or later** (required for MCP support)
- MCP Time Server built and ready to run

### Configuration Steps

#### 1. Create the Configuration File

Create a `.mcp.json` file in one of these locations:

- **Solution-specific** (recommended): `<SOLUTIONDIR>\.mcp.json` (tracked in source control)
- **Visual Studio only**: `<SOLUTIONDIR>\.vs\mcp.json` 
- **User-wide**: `%USERPROFILE%\.mcp.json`

#### 2. Add Server Configuration

For this StdIO-based MCP server, add the following configuration to `.mcp.json`:

```json
{
  "mcpServers": {
    "time-server": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "path/to/Dotnet10Samples/src/MCPTimeServer/MCPTimeServer.csproj"
      ]
    }
  }
}
```

**Important**: Update the path to match your actual project location.

#### Alternative: Using Published Executable

If you've published the server:

```json
{
  "mcpServers": {
    "time-server": {
      "command": "path/to/MCPTimeServer.exe"
    }
  }
}
```

#### 3. Example Configuration for Development

Here's a complete example for a development setup:

```json
{
  "mcpServers": {
    "time-server": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:\\Dev\\Dotnet10Samples\\src\\MCPTimeServer\\MCPTimeServer.csproj",
        "--configuration",
        "Release"
      ]
    }
  }
}
```

### Using the MCP Server in Visual Studio Copilot

1. **Start Visual Studio** and open your solution
2. **Open Copilot Chat** (`Ctrl+/`)
3. **Select Agent Mode** - Click the agent dropdown in Copilot Chat
4. The MCP Time Server tools will be available:
   - `GetLocalTime` - Query current local time
   - `GetCityTime` - Query time in specific cities
   - `ListSupportedCities` - See all available cities
   - `CompareTime` - Compare times between cities

### Example Queries in Copilot Chat

Once configured, you can ask Copilot questions like:

```
"What time is it in Tokyo?"
"Compare the time between Vienna and New York"
"List all supported cities in the time server"
"What's my current local time?"
```

Copilot will automatically invoke the appropriate MCP tools to answer these questions.

### Troubleshooting

**Server not appearing in Copilot:**
- Verify the `.mcp.json` file is in the correct location
- Check that the path to the project is correct
- Restart Visual Studio after creating/editing `.mcp.json`

**Permission errors:**
- Ensure the project has been built at least once
- Check that .NET 10 SDK is in your PATH

**Server fails to start:**
- Test the server manually with `dotnet run` first
- Check the Output window in Visual Studio for error messages

### Security Note

MCP servers can execute code on your machine. Only configure MCP servers from trusted sources.

## 📝 Code Examples

### Example 1: Adding a New City

Edit `TimeTools.cs` and add to the `CityTimezones` dictionary:

```csharp
private static readonly Dictionary<string, string> CityTimezones = new()
{
    // ... existing cities ...
    ["Cairo"] = "Africa/Cairo",
    ["Istanbul"] = "Europe/Istanbul",
    ["Bangkok"] = "Asia/Bangkok"
};
```

### Example 2: Creating a Custom Tool

Add a new tool method to the `TimeTools` class:

```csharp
/// <summary>
/// Gets the current UTC time
/// </summary>
/// <returns>Current UTC time</returns>
[McpServerTool]
[System.ComponentModel.Description("Get the current UTC time")]
public static string GetUtcTime()
{
    Instant now = SystemClock.Instance.GetCurrentInstant();
    return $"Current UTC Time: {now:yyyy-MM-dd HH:mm:ss} UTC";
}
```

### Example 3: Using Modern C# Features

The project demonstrates several modern C# features:

#### Collection Expressions (C# 12)
```csharp
List<string> cities = [.. CityTimezones.Keys.Order()];
```

#### Raw String Literals (C# 11)
```csharp
return $"""
    Current Time in {location}:
    Time: {cityTime:F}
    Timezone: {timezoneId}
    """;
```

#### Pattern Matching
```csharp
if (CityTimezones.TryGetValue(location, out string? mappedTimezone))
{
    timezoneId = mappedTimezone;
}
```

## 🧪 Testing the Server

### Manual Testing

Since MCP servers communicate via StdIO, testing requires an MCP client. However, you can verify the server builds and runs:

```bash
# Build test
dotnet build

# Run test (server will start)
dotnet run
```

### Integration with MCP Clients

To use with an MCP client (like Claude Desktop or GitHub Copilot):

1. **Package the server** (future enhancement):
```bash
dotnet publish -c Release
```

2. **Configure in MCP client** with the path to the executable

3. **Use the tools** through natural language queries:
   - "What time is it in Tokyo?"
   - "Compare time between London and New York"
   - "List all supported cities"

## 🏗️ Project Structure

```
MCPTimeServer/
├── MCPTimeServer.csproj    # Project file with dependencies
├── Program.cs              # MCP server host configuration
├── TimeTools.cs            # MCP tools implementation
└── README.md              # This file
```

### Key Files

#### `MCPTimeServer.csproj`
- Targets .NET 10 (`net10.0`)
- Uses C# preview language features
- References MCP SDK and NodaTime

#### `Program.cs`
- Configures MCP server hosting
- Sets up StdIO transport
- Auto-discovers tools from assembly

#### `TimeTools.cs`
- Defines MCP tools with `[McpServerTool]` attribute
- Implements time query logic
- Uses NodaTime for robust timezone handling

## 🔍 How It Works

### MCP Protocol Overview

Model Context Protocol (MCP) is a standardized protocol for AI assistants to interact with external tools and services. This server:

1. **Exposes Tools**: Methods marked with `[McpServerTool]` are exposed to MCP clients
2. **Handles Requests**: Receives tool invocation requests via StdIO
3. **Returns Results**: Sends formatted responses back to the client

### Tool Discovery

The MCP SDK automatically discovers tools using:

```csharp
builder.Services
    .AddMcpServer()
    .WithToolsFromAssembly();  // Scans for [McpServerToolType] and [McpServerTool]
```

### Timezone Handling

The server uses **NodaTime** for accurate timezone operations:

- **IANA Timezone Database**: Standard timezone identifiers (e.g., "America/New_York")
- **Instant**: Represents a specific moment in time
- **ZonedDateTime**: Instant in a specific timezone
- **DateTimeZone**: Timezone information and rules

## 🚧 Extending the Server

### Adding New Features

1. **Create new tool method** in `TimeTools.cs`
2. **Add attributes**: `[McpServerTool]` and `[Description]`
3. **Implement logic** using NodaTime
4. **Build and test**

### Example Extensions

- **Sunrise/Sunset Times**: Add astronomical calculations
- **World Clock**: Display multiple cities simultaneously
- **Timezone Converter**: Convert specific times between zones
- **Meeting Planner**: Find optimal meeting times across timezones
- **Holiday Calendar**: Include timezone-aware holiday information

### Best Practices

1. **Use NodaTime** for all timezone operations (avoids DateTime pitfalls)
2. **Validate inputs** and provide helpful error messages
3. **Document tools** with clear descriptions
4. **Handle edge cases** (invalid timezones, null inputs, etc.)
5. **Use modern C# features** (collection expressions, pattern matching, raw strings)

## 📚 Learning Resources

### MCP Resources
- [MCP Documentation](https://modelcontextprotocol.io/)
- [.NET MCP SDK Documentation](https://learn.microsoft.com/en-us/dotnet/core/mcp/)
- [MCP C# SDK GitHub](https://github.com/modelcontextprotocol/csharp-sdk)
- [MCP Server Publishing Guide](https://erikej.github.io/mcp/dotnet/nuget/2025/07/30/mcp-dotnet-nuget.html)

### .NET 10 Resources
- [.NET 10 Documentation](https://docs.microsoft.com/dotnet/core/whats-new/dotnet-10)
- [C# 14 Language Features](https://docs.microsoft.com/dotnet/csharp)
- [Visual Studio 2024 Documentation](https://docs.microsoft.com/visualstudio/)

### GitHub Copilot Resources
- [GitHub Copilot in Visual Studio](https://learn.microsoft.com/en-us/visualstudio/ide/github-copilot)
- [Copilot Best Practices](https://github.blog/developer-skills/github/how-to-use-github-copilot-in-your-ide-tips-tricks-and-best-practices/)
- [Visual Studio 2026 Copilot Guide](../../COPILOT_VISUAL_STUDIO_2026_GUIDE.md)

### Timezone Resources
- [NodaTime Documentation](https://nodatime.org/)
- [IANA Time Zone Database](https://www.iana.org/time-zones)
- [List of tz Database Time Zones](https://en.wikipedia.org/wiki/List_of_tz_database_time_zones)

## 🛠️ Troubleshooting

### Build Issues

**Problem**: "The type or namespace name 'ModelContextProtocol' could not be found"

**Solution**: Ensure preview packages are enabled:
```bash
dotnet restore --force
dotnet build
```

**Problem**: ".NET 10 SDK not found"

**Solution**: Install .NET 10 SDK:
- Download from https://dotnet.microsoft.com/download/dotnet/10.0
- Verify with `dotnet --version`

### Runtime Issues

**Problem**: Server starts but tools aren't discovered

**Solution**: Verify `[McpServerToolType]` attribute is on the class:
```csharp
[McpServerToolType]
public static class TimeTools
```

**Problem**: Timezone not found

**Solution**: Use IANA timezone identifiers (e.g., "America/New_York", not "EST")

## 🤝 Contributing

Contributions are welcome! To contribute:

1. Fork the repository
2. Create a feature branch
3. Add your changes with modern C# features
4. Test thoroughly
5. Submit a pull request

## 📄 License

This sample is part of the .NET 10 Samples Collection and is licensed under the MIT License.

## ✨ Acknowledgments

- **Model Context Protocol**: For the standardized AI tool protocol
- **NodaTime**: For robust timezone handling
- **.NET Team**: For .NET 10 and MCP SDK preview
- **GitHub Copilot**: For AI-assisted development capabilities

---

**Built with .NET 10, MCP SDK Preview, and Modern C# Features 🚀**

**Powered by NodaTime for Accurate Timezone Operations 🌍**
