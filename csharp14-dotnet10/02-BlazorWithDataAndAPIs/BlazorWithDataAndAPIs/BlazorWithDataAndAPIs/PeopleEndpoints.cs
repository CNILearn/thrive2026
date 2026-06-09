using BlazorWithDataAndAPIs.Client.Models;
using BlazorWithDataAndAPIs.Client.Services;
using BlazorWithDataAndAPIs.Data;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BlazorWithDataAndAPIs;

public static class PeopleEndpoints
{
    public static void MapPersonEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Person").WithTags(nameof(Person));
        group.MapGet("/createdb", async Task<Ok<string>> (IDbContextFactory<EmployeesContext> dbContextFactory) =>
        {
            await using var db = await dbContextFactory.CreateDbContextAsync();
            await db.Database.EnsureCreatedAsync();
            return TypedResults.Ok("Database created successfully.");
        });

        group.MapGet("/", async (IEmployeeService employeeService) =>
        {
            var people = await employeeService.GetAllPeopleAsync();
            return TypedResults.Ok(people);
            //return await db.People.ToListAsync();
        })
        .WithName("GetAllPeople");

        group.MapGet("/{id}", async Task<Results<Ok<Person>, NotFound>> (int id, IEmployeeService employeeService) =>
        {
            var person = await employeeService.GetPersonById(id);
            return person is Person model
                ? TypedResults.Ok(model)
                : TypedResults.NotFound();
        })
        .WithName("GetPersonById");

        //group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Person person, EmployeesContext db) =>
        //{
        //    var affected = await db.People
        //        .Where(model => model.Id == id)
        //        .ExecuteUpdateAsync(setters => setters
        //            .SetProperty(m => m.Id, person.Id)
        //            .SetProperty(m => m.FirstName, person.FirstName)
        //            .SetProperty(m => m.LastName, person.LastName)
        //            .SetProperty(m => m.MiddleName, person.MiddleName)
        //            .SetProperty(m => m.Email, person.Email)
        //            );
        //    return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        //})
        //.WithName("UpdatePerson");

        //group.MapPost("/", async (Person person, EmployeesContext db) =>
        //{
        //    db.People.Add(person);
        //    await db.SaveChangesAsync();
        //    return TypedResults.Created($"/api/Person/{person.Id}",person);
        //})
        //.WithName("CreatePerson");

        //group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, EmployeesContext db) =>
        //{
        //    var affected = await db.People
        //        .Where(model => model.Id == id)
        //        .ExecuteDeleteAsync();
        //    return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        //})
        //.WithName("DeletePerson");
    }
}
