using System.Xml;

namespace Lab2OOP;

public class DomStrategy : IAnalysisStrategy
{
    public List<BookResult> Search(SearchCriteria criteria, Stream xmlStream)
    {
        var results = new List<BookResult>();
        XmlDocument doc = new XmlDocument();
        doc.Load(xmlStream);

        string xpath = "//book";
        var conditions = new List<string>();

        if (!string.IsNullOrWhiteSpace(criteria.Genre))
        {
            conditions.Add($"@genre='{criteria.Genre}'");
        }
        if (!string.IsNullOrWhiteSpace(criteria.Author))
        {
            conditions.Add($"author[contains(., '{criteria.Author}')]");
        }
        if (!string.IsNullOrWhiteSpace(criteria.Faculty))
        {
            conditions.Add($"author/@faculty='{criteria.Faculty}'");
        }

        if (conditions.Count > 0)
        {
            xpath += $"[{string.Join(" and ", conditions)}]";
        }

        XmlNodeList nodes = doc.SelectNodes(xpath);

        foreach (XmlNode node in nodes)
        {
            var book = new BookResult();
            book.Title = node.SelectSingleNode("title").InnerText ?? "Без назви";
            book.Genre = node.Attributes["genre"]?.Value ?? "Невідомо";
            book.Year = node.SelectSingleNode("year")?.InnerText ?? "Невідомо";

            foreach (XmlNode authorNode in node.SelectNodes("author"))
            {
                string name = authorNode.InnerText;
                string faculty = authorNode.Attributes["faculty"]?.Value ?? "Невідомо";
                book.AuthorsInfo.Add($"{name} ({faculty})");
            }
            results.Add(book);
        }
        return results;
    }
}