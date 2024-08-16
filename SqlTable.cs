using System.Text.RegularExpressions;

namespace CaseStudy;

public class SqlTable
{
    public string Name { get; }

    public SqlTable(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var isMatch = Regex.IsMatch(name, "^customers_([\\w]{2})$");
        if (!isMatch)
        {
            throw new ArgumentException("Input not matching: ^customers_([\\\\w]{2})$");
        }

        Name = name;
    }
}