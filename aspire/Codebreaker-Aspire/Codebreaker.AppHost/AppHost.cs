var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .AddDatabase("Games");

var gameApis = builder.AddProject<Projects.Codebreaker_GameAPIs>("codebreaker-gameapis")
    .WithReference(sql)
    .WaitFor(sql);

builder.AddProject<Projects.CodeBreaker_Bot>("codebreaker-bot")
    .WithReference(gameApis)
    .WaitFor(gameApis);

builder.Build().Run();
