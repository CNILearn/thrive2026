using BlazorWithDataAndAPIs.Client.Models;

using System.Net.Http.Json;
using System.Text.Json;

namespace BlazorWithDataAndAPIs.Client.Services;

public class EmployeeClientService(HttpClient client) : IEmployeeService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    public async Task<Person> CreatePersonAsync(Person person)
    {
        await client.PostAsJsonAsync("/api/Person", person, _jsonOptions);
        return person;
    }

    public async Task<IEnumerable<Person>> GetAllPeopleAsync()
    {
        var people = await client.GetFromJsonAsync<IEnumerable<Person>>("/api/Person", _jsonOptions);
        if (people == null) throw new HttpRequestException();

        return people;
    }

    public async Task<Person?> GetPersonById(int id)
    {
        var person = await client.GetFromJsonAsync<Person>("/api/People/{id}", _jsonOptions);
        return person;
    }
}
