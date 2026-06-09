using BlazorWithDataAndAPIs.Client.Services;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);


builder.Services.AddHttpClient<IEmployeeService, EmployeeClientService>(client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});
// builder.Services.AddTransient<IEmployeeService, EmployeeClientService>();

await builder.Build().RunAsync();
