using BlazorWithDataAndAPIs.Client.Models;

using Microsoft.EntityFrameworkCore;

namespace BlazorWithDataAndAPIs.Data;

public class EmployeesContext(DbContextOptions<EmployeesContext> options) : DbContext(options)
{
    public DbSet<Person> People => Set<Person>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.MiddleName).HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired(false).HasMaxLength(100);
            entity.HasData(GetSamplePeople());
        });
    }

    private static Person[] GetSamplePeople() =>
    [
        new() { Id =  1, FirstName = "Clark",    LastName = "Kent",      Email = "clark.kent@dailyplanet.com" },
        new() { Id =  2, FirstName = "Bruce",    LastName = "Wayne",     Email = "bruce.wayne@wayneenterprises.com" },
        new() { Id =  3, FirstName = "Diana",    LastName = "Prince",    Email = "diana.prince@justice.org" },
        new() { Id =  4, FirstName = "Barry",    LastName = "Allen",     Email = "barry.allen@ccpd.gov" },
        new() { Id =  5, FirstName = "Hal",      LastName = "Jordan",    Email = "hal.jordan@ferrisair.com" },
        new() { Id =  6, FirstName = "Arthur",   LastName = "Curry",     Email = "arthur.curry@atlantis.gov" },
        new() { Id =  7, FirstName = "Victor",   LastName = "Stone",     Email = "victor.stone@justice.org" },
        new() { Id =  8, FirstName = "Peter",    LastName = "Parker",    Email = "peter.parker@bugle.com" },
        new() { Id =  9, FirstName = "Tony",     LastName = "Stark",     Email = "tony.stark@starkindustries.com" },
        new() { Id = 10, FirstName = "Steve",    LastName = "Rogers",    Email = "steve.rogers@shield.gov" },
        new() { Id = 11, FirstName = "Natasha",  LastName = "Romanoff",  Email = "natasha.romanoff@shield.gov" },
        new() { Id = 12, FirstName = "Bruce",    LastName = "Banner",    Email = "bruce.banner@shield.gov" },
        new() { Id = 13, FirstName = "Thor",     LastName = "Odinson",   Email = "thor.odinson@asgard.org" },
        new() { Id = 14, FirstName = "Wanda",    LastName = "Maximoff",  Email = "wanda.maximoff@avengers.org" },
        new() { Id = 15, FirstName = "Scott",    LastName = "Summers",   Email = "scott.summers@xavierinstitute.edu" },
        new() { Id = 16, FirstName = "Jean",     LastName = "Grey",      Email = "jean.grey@xavierinstitute.edu" },
        new() { Id = 17, FirstName = "Wade",     LastName = "Wilson",    Email = "wade.wilson@mercforhire.com" },
        new() { Id = 18, FirstName = "Matt",     LastName = "Murdock",   Email = "matt.murdock@murdocklaw.com" },
        new() { Id = 19, FirstName = "Oliver",   LastName = "Queen",     Email = "oliver.queen@queenconsolidated.com" },
        new() { Id = 20, FirstName = "Selina",   LastName = "Kyle",      Email = "selina.kyle@gothamjewelers.com" },
    ];
}
