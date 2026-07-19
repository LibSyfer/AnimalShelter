var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgWeb(pgWeb => pgWeb.WithHostPort(5050));
var postgresdb = postgres.AddDatabase("animalshelterdb");

var minioUser = builder.AddParameter("minio-user", "minioadmin");
var minioPassword = builder.AddParameter("minio-password", "minioadmin", secret: true);
var minio = builder.AddContainer("minio", "minio/minio")
    .WithEnvironment("MINIO_ROOT_USER", minioUser)
    .WithEnvironment("MINIO_ROOT_PASSWORD", minioPassword)
    .WithArgs("server", "/data", "--console-address", ":9001")
    .WithEndpoint(port: 9000, targetPort: 9000, scheme: "http", name: "api")
    .WithEndpoint(port: 9001, targetPort: 9001, scheme: "http", name: "console")
    .WithVolume("minio-data", "/data");

var migrationService = builder.AddProject<Projects.AnimalShelter_MigrationService>("animalshelter-migrationservice")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.AddProject<Projects.AnimalShelter_Api>("animalshelter-api")
    .WithReference(postgresdb)
    .WaitForCompletion(migrationService)
    //.WithEnvironment("S3Storage__Endpoint", minio.GetEndpoint("api"))
    .WithEnvironment("S3Storage__AccessKey", minioUser)
    .WithEnvironment("S3Storage__SecretKey", minioPassword)
    .WaitFor(minio); ;

builder.Build().Run();
