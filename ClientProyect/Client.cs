using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration 
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>();

var clientTransport = new StdioClientTransport(new()
{
    Name = "Demo Server",
    Command = "dotnet",
    Arguments = new[]
    {
        @"C:\MCP_Proyectos\ConsoleApp1\bin\Debug\net8.0\McpServer.dll"
    }
});

Console.WriteLine("Setting up stdio transport");

await using var mcpClient = await McpClientFactory.CreateAsync(clientTransport);

Console.WriteLine("Listing tools");

var tools = await mcpClient.ListToolsAsync();

foreach (var tool in tools)
{
    Console.WriteLine($"Connected to server with tools: {tool.Name}");
}

var result = await mcpClient.CallToolAsync(
    "add",
    new Dictionary<string, object?>() { ["a"] = 1, ["b"] = 3  },
    cancellationToken:CancellationToken.None);

Console.WriteLine("Result: " + ((TextContentBlock)result.Content[0]).Text);