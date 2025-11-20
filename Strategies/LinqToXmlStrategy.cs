using System.Xml.Linq;

namespace Lab2OOP;

public class LinqToXmlStrategy : IAnalysisStrategy
{
    public List<string> Search(SearchCriteria criteria, Stream xmlStream)
    {
        XDocument doc = XDocument.Load(xmlStream);

        var query = from book in doc.Descendants("book")
                    where (string.IsNullOrWhiteSpace(criteria.Genre) ||
                           (string)book.Attribute("genre") == criteria.Genre)
                    where (string.IsNullOrWhiteSpace(criteria.Author) ||
                           book.Elements("author").Any(a => a.Value.Contains(criteria.Author, StringComparison.OrdinalIgnoreCase)))
                    where (string.IsNullOrWhiteSpace(criteria.Faculty) ||
                           book.Elements("author").Any(a => (string)a.Attribute("faculty") == criteria.Faculty))
                    select book.Element("title").Value;

        return query.ToList();
    }
}