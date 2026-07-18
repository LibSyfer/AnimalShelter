var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgWeb(pgWeb => pgWeb.WithHostPort(5050));

var postgresdb = postgres.AddDatabase("animalshelterdb");

var migrationService = builder.AddProject<Projects.AnimalShelter_MigrationService>("animalshelter-migrationservice")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.AddProject<Projects.AnimalShelter_Api>("animalshelter-api")
    .WithReference(postgresdb)
    .WaitForCompletion(migrationService);

builder.Build().Run();
