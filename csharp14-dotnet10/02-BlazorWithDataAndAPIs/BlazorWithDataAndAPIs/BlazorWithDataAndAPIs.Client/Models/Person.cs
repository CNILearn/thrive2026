using System.ComponentModel.DataAnnotations;

namespace BlazorWithDataAndAPIs.Client.Models;

public class Person
{
    public int Id { get; set; }

    [StringLength(50)]
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? Email { get; set; }
}
