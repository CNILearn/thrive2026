using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Server;
using MCPTimeServer;

// Build the MCP server host
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// Configure MCP server with tools
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()  // Use StdIO transport for CLI-based communication
    .WithToolsFromAssembly();     // Auto-discover tools with [McpServerToolType] attribute

// Build and run the server
IHost host = builder.Build();

Console.WriteLine("🕐 MCP Time Server Starting...");
Console.WriteLine("Ready to handle time queries via Model Context Protocol");
Console.WriteLine();

await host.RunAsync();
