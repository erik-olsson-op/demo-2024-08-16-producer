using System.Globalization;
using CsvHelper;

namespace CaseStudy;

public class CsvHelper
{
    public static string Write(SqlTable sqlTable, IEnumerable<Customer> customers)
    {
        var path = sqlTable.Name + ".csv";
        using var writer = new StreamWriter(path);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(customers);
        return path;
    }
}