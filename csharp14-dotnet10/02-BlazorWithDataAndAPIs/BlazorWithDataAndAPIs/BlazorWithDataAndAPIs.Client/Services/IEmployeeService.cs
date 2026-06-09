using BlazorWithDataAndAPIs.Client.Models;

namespace BlazorWithDataAndAPIs.Client.Services;

public interface IEmployeeService
{
    Task<IEnumerable<Person>> GetAllPeopleAsync();
    Task<Person?> GetPersonById(int id);
    Task<Person> CreatePersonAsync(Person person);
}
