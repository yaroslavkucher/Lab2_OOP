namespace Lab2OOP;
public interface IAnalysisStrategy
{
    List<string> Search(SearchCriteria criteria, Stream xmlStream);
}