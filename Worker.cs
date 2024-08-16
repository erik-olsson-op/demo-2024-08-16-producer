using System.Net;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.IdentityModel.Tokens;

namespace CaseStudy;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IDatabaseService _databaseService;
    private readonly int _interval;
    private readonly SqlTable _sqlTable;
    private readonly AwsOption _awsOption;

    public Worker(ILogger<Worker> logger,
        IDatabaseService databaseService,
        int interval,
        SqlTable sqlTable,
        AwsOption awsOption)
    {
        _logger = logger;
        _databaseService = databaseService;
        _interval = interval;
        _sqlTable = sqlTable;
        _awsOption = awsOption;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var customers = _databaseService.Get(_sqlTable).ToList();
                if (!customers.IsNullOrEmpty())
                {
                    var csvPath = CsvHelper.Write(_sqlTable, customers);
                    var client =
                        new AmazonS3Client(new BasicAWSCredentials(_awsOption.AccessKey, _awsOption.SecretKey),
                            RegionEndpoint.EUNorth1);
                    var request = new PutObjectRequest
                    {
                        BucketName = _awsOption.BucketName,
                        Key = _sqlTable.Name,
                        FilePath = csvPath,
                    };

                    var response = await client.PutObjectAsync(request, stoppingToken);
                    if (response.HttpStatusCode == HttpStatusCode.OK)
                    {
                        _logger.LogInformation($"Successfully uploaded {_sqlTable.Name} to {_awsOption.BucketName}.");
                        _databaseService.Update(_sqlTable, customers);
                    }
                    else
                    {
                        _logger.LogInformation($"Failed upload {_sqlTable.Name} to {_awsOption.BucketName}.");
                    }
                }
                else
                {
                    _logger.LogInformation("Empty: {Name}", _sqlTable.Name);
                }

                await Task.Delay(TimeSpan.FromSeconds(_interval), stoppingToken);
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