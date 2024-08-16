using Bogus;

namespace CaseStudy;

public class DatabaseSeeder : BackgroundService
{
    private readonly ILogger<DatabaseSeeder> _logger;
    private readonly IDatabaseService _databaseService;
    private readonly IEnumerable<SqlTable> _sqlTables;

    public DatabaseSeeder(ILogger<DatabaseSeeder> logger,
        IDatabaseService databaseService,
        IEnumerable<SqlTable> sqlTables)
    {
        _logger = logger;
        _databaseService = databaseService;
        _sqlTables = sqlTables;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                foreach (var sqlTable in _sqlTables)
                {
                    _logger.LogInformation("Seeding: {Name}", sqlTable.Name);
                    var customerFaker = new Faker<Customer>()
                        .RuleFor(u => u.Name, f => f.Person.FullName)
                        .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber());

                    var customers = customerFaker.Generate(50);
                    _databaseService.Insert(sqlTable, customers);
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception e)
            {
                _logger.LogError(e.StackTrace, e);
                Environment.Exit(1);
            }
        }
    }
}