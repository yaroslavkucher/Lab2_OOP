namespace Lab2OOP;
public interface IAnalysisStrategy
{
    List<BookResult> Search(SearchCriteria criteria, Stream xmlStream);
}