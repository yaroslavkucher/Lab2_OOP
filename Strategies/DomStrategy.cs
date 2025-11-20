using System.Xml;

namespace Lab2OOP;

public class DomStrategy : IAnalysisStrategy
{
    public List<string> Search(SearchCriteria criteria, Stream xmlStream)
    {
        var results = new List<string>();
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
            results.Add(node.SelectSingleNode("title").InnerText);
        }
        return results;
    }
}