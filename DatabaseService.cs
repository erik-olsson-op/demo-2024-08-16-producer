using System.Data;
using Microsoft.Data.SqlClient;

namespace CaseStudy;

public class DatabaseService : IDatabaseService
{
    private readonly ILogger<IDatabaseService> _Logger;
    private readonly string ConnectionString;

    public DatabaseService(ILogger<IDatabaseService> logger, string connectionString)
    {
        _Logger = logger;
        ConnectionString = connectionString;
    }

    public void Insert(SqlTable sqlTable, IEnumerable<Customer> customers)
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();
        using var batch = new SqlBatch(connection);
        foreach (var customer in customers)
        {
            var batchCommand = batch.CreateBatchCommand();
            batchCommand.CommandText = $"""
                                        INSERT INTO {sqlTable.Name} (Name, Phone, Uploaded)
                                        VALUES (@Name, @Phone, @Uploaded)
                                        """;
            batchCommand.CommandType = CommandType.Text;
            var Nameparam = batchCommand.CreateParameter();
            Nameparam.ParameterName = "@Name";
            Nameparam.Value = customer.Name;
            batchCommand.Parameters.Add(Nameparam);
            var phoneParam = batchCommand.CreateParameter();
            phoneParam.ParameterName = "@Phone";
            phoneParam.Value = customer.PhoneNumber;
            batchCommand.Parameters.Add(phoneParam);
            var uploadedParam = batchCommand.CreateParameter();
            uploadedParam.ParameterName = "@Uploaded";
            uploadedParam.Value = customer.Uploaded;
            batchCommand.Parameters.Add(uploadedParam);
            batch.BatchCommands.Add(batchCommand);
        }

        batch.ExecuteNonQuery();
    }

    public void Update(SqlTable sqlTable, IEnumerable<Customer> customers)
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();
        using var batch = new SqlBatch(connection);
        foreach (var customer in customers)
        {
            var batchCommand = batch.CreateBatchCommand();
            batchCommand.CommandText = $"""
                                        UPDATE {sqlTable.Name}
                                        SET Uploaded = 1
                                        WHERE Id = @Id
                                        """;
            batchCommand.CommandType = CommandType.Text;
            var IdParam = batchCommand.CreateParameter();
            IdParam.ParameterName = "@Id";
            IdParam.Value = customer.Id;
            batchCommand.Parameters.Add(IdParam);
            batch.BatchCommands.Add(batchCommand);
        }

        batch.ExecuteNonQuery();
    }

    public IEnumerable<Customer> Get(SqlTable sqlTable)
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();
        var query = $"""
                     SELECT 
                     *
                     FROM
                     {sqlTable.Name}
                     WHERE Uploaded = 0
                     """;
        using var command = new SqlCommand(query, connection);
        using var reader = command.ExecuteReader();
        var l = new List<Customer>();
        while (reader.Read())
        {
            int id = (int)reader["id"];
            string name = (string)reader["name"];
            string phone = (string)reader["phone"];
            bool uploaded = (bool)reader["uploaded"];
            var c = new Customer()
            {
                Id = id,
                Name = name,
                PhoneNumber = phone,
                Uploaded = uploaded
            };
            l.Add(c);
        }

        return l;
    }
}