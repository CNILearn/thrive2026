using BlazorWithDataAndAPIs;
using BlazorWithDataAndAPIs.Client.Services;
using BlazorWithDataAndAPIs.Components;
using BlazorWithDataAndAPIs.Data;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDbContextFactory<EmployeesContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("employees")
        ?? throw new InvalidOperationException("Connection string 'employees' not found.")));
builder.Services.AddScoped<IEmployeeService, EmployeeDbService>();

builder.EnrichNpgsqlDbContext<EmployeesContext>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
};

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorWithDataAndAPIs.Client._Imports).Assembly);

app.MapPersonEndpoints();

app.Run();
