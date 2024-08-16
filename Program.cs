using CaseStudy;
using Microsoft.Data.SqlClient;

var builder = Host.CreateApplicationBuilder(args);
// Configure Runtime for Window Service
builder.Services.AddWindowsService(options => { options.ServiceName = ".NET AWS CaseStudy Omegapoint"; });

// Register Service Layer in DI container
builder.Services.AddSingleton<IDatabaseService>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<IDatabaseService>>();
    var databaseOption = builder.Configuration
        .GetSection(nameof(DatabaseOption))
        .Get<DatabaseOption>();
    var sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
    sqlConnectionStringBuilder.DataSource = databaseOption.Host;
    sqlConnectionStringBuilder.InitialCatalog = databaseOption.Schema;
    sqlConnectionStringBuilder.UserID = databaseOption.Username;
    sqlConnectionStringBuilder.Password = databaseOption.Password;
    sqlConnectionStringBuilder.TrustServerCertificate = true;
    sqlConnectionStringBuilder.CommandTimeout = 300;
    sqlConnectionStringBuilder.ApplicationName = ".NET AWS CaseStudy Omegapoint";
    sqlConnectionStringBuilder.ApplicationIntent = ApplicationIntent.ReadWrite;
    return new DatabaseService(logger, sqlConnectionStringBuilder.ConnectionString);
});

// Add Workers to the Runtime

// Add Database Seeder
builder.Services.AddSingleton<IHostedService>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<DatabaseSeeder>>();
    var db = provider.GetRequiredService<IDatabaseService>();
    var workerOption = builder.Configuration
        .GetSection(nameof(WorkerOption))
        .Get<WorkerOption>();
    var l = workerOption!.SqlTableNames
        .ConvertAll(input => new SqlTable(input));
    return new DatabaseSeeder(logger, db, l);
});
// Add Sweden
builder.Services.AddSingleton<IHostedService>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<Worker>>();
    var db = provider.GetRequiredService<IDatabaseService>();
    var workerOption = builder.Configuration
        .GetSection(nameof(WorkerOption))
        .Get<WorkerOption>();
    var interval = workerOption.Interval;
    var sqlTable = workerOption!.SqlTableNames
        .ConvertAll(input => new SqlTable(input))
        .Find(table => table.Name.EndsWith("SE"));
    var awsOption = builder.Configuration
        .GetSection(nameof(AwsOption))
        .Get<AwsOption>();
    return new Worker(logger, db, interval, sqlTable!, awsOption!);
});
// Add Finland
builder.Services.AddSingleton<IHostedService>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<Worker>>();
    var db = provider.GetRequiredService<IDatabaseService>();
    var workerOption = builder.Configuration
        .GetSection(nameof(WorkerOption))
        .Get<WorkerOption>();
    var interval = workerOption.Interval;
    var sqlTable = workerOption!.SqlTableNames
        .ConvertAll(input => new SqlTable(input))
        .Find(table => table.Name.EndsWith("FI"));
    var awsOption = builder.Configuration
        .GetSection(nameof(AwsOption))
        .Get<AwsOption>();
    return new Worker(logger, db, interval, sqlTable!, awsOption!);
});

var host = builder.Build();
host.Run();