var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050));

var postgresdb = postgres.AddDatabase("postgresdb");

builder.AddProject<Projects.AnimalShelter_Api>("animalshelter-api")
    .WithReference(postgresdb);

builder.Build().Run();
