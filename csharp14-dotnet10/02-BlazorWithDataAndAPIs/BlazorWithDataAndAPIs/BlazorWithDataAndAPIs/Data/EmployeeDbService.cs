using BlazorWithDataAndAPIs.Client.Models;
using BlazorWithDataAndAPIs.Client.Services;

using Microsoft.EntityFrameworkCore;

namespace BlazorWithDataAndAPIs.Data;

internal sealed class EmployeeDbService(IDbContextFactory<EmployeesContext> contextFactory) : IEmployeeService
{
    private readonly IDbContextFactory<EmployeesContext> _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));

    public async Task<IEnumerable<Person>> GetAllPeopleAsync()
    {
        await using var db = await _contextFactory.CreateDbContextAsync();
        return await db.People.AsNoTracking().ToListAsync();
    }

    public async Task<Person?> GetPersonById(int id)
    {
        await using var db = await _contextFactory.CreateDbContextAsync();
        return await db.People.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Person> CreatePersonAsync(Person person)
    {
        ArgumentNullException.ThrowIfNull(person);

        await using var db = await _contextFactory.CreateDbContextAsync();
        db.People.Add(person);
        await db.SaveChangesAsync();
        return person;
    }
}
