var builder = DistributedApplication.CreateBuilder(args);

//var sqlite = builder.AddSqlite("sqlite-employees")
//   .WithSqliteWeb();

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume("postgres-volume", isReadOnly: false)
    .WithPgAdmin();

var db = postgres.AddDatabase("employees");

var blazor = builder.AddProject<Projects.BlazorWithDataAndAPIs>("blazor")
    .WithReference(db).WaitFor(db);

builder.Build().Run();
