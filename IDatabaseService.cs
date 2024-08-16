namespace CaseStudy;

public interface IDatabaseService
{
    void Insert(SqlTable sqlTable, IEnumerable<Customer> customers);

    void Update(SqlTable sqlTable, IEnumerable<Customer> customers);

    IEnumerable<Customer> Get(SqlTable sqlTable);
}