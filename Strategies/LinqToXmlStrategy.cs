using System.Xml.Linq;

namespace Lab2OOP;

public class LinqToXmlStrategy : IAnalysisStrategy
{
    public List<BookResult> Search(SearchCriteria criteria, Stream xmlStream)
    {
        XDocument doc = XDocument.Load(xmlStream);

        var query = from book in doc.Descendants("book")
                    where (string.IsNullOrWhiteSpace(criteria.Genre) ||
                           (string)book.Attribute("genre") == criteria.Genre)
                    where (string.IsNullOrWhiteSpace(criteria.Author) ||
                           book.Elements("author").Any(a => a.Value.Contains(criteria.Author, StringComparison.OrdinalIgnoreCase)))
                    where (string.IsNullOrWhiteSpace(criteria.Faculty) ||
                           book.Elements("author").Any(a => (string)a.Attribute("faculty") == criteria.Faculty))
                    select new BookResult
                    {
                        Title = (string)book.Element("title"),
                        Genre = (string)book.Attribute("genre"),
                        Year = (string)book.Element("year"),
                        AuthorsInfo = book.Elements("author")
                                          .Select(a => $"{a.Value} (Faculty: {(string)a.Attribute("faculty")})")
                                          .ToList()
                    };

        return query.ToList();
    }
}