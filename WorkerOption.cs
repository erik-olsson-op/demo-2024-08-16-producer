namespace CaseStudy;

public class WorkerOption
{
    public required int Interval { get; init; }
    public required List<string> SqlTableNames { get; init; }
}