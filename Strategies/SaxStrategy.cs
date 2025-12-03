using System.Xml;

namespace Lab2OOP;

public class SaxStrategy : IAnalysisStrategy
{
    public List<BookResult> Search(SearchCriteria criteria, Stream xmlStream)
    {
        var results = new List<BookResult>();
        using (XmlReader reader = XmlReader.Create(xmlStream))
        {
            BookResult currentBook = null;
            string currentElement = "";

            bool matchGenre = true;
            bool matchAuthor = true;
            bool matchFaculty = true;

            bool hasAuthorFilter = !string.IsNullOrWhiteSpace(criteria.Author);
            bool hasFacultyFilter = !string.IsNullOrWhiteSpace(criteria.Faculty);

            string tempAuthorName = null;
            string tempAuthorFaculty = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    currentElement = reader.Name;

                    if (currentElement == "book")
                    {
                        currentBook = new BookResult();

                        string genre = reader.GetAttribute("genre");
                        currentBook.Genre = genre;
                        if (!string.IsNullOrWhiteSpace(criteria.Genre))
                        {
                            matchGenre = string.Equals(genre, criteria.Genre, StringComparison.OrdinalIgnoreCase);
                        }
                        else
                        {
                            matchGenre = true;
                        }

                        matchAuthor = !hasAuthorFilter;
                        matchFaculty = !hasFacultyFilter;
                    }
                    else if (currentElement == "author")
                    {
                        tempAuthorFaculty = reader.GetAttribute("faculty") ?? "";

                        if (hasFacultyFilter && !matchFaculty)
                        {
                            if (string.Equals(tempAuthorFaculty, criteria.Faculty, StringComparison.OrdinalIgnoreCase))
                                matchFaculty = true;
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.Text)
                {
                    if (currentBook != null)
                    {
                        if (currentElement == "title")
                            currentBook.Title = reader.Value;
                        else if (currentElement == "year")
                            currentBook.Year = reader.Value;
                        else if (currentElement == "author")
                        {
                            tempAuthorName = reader.Value;

                            currentBook.AuthorsInfo.Add($"{tempAuthorName} ({tempAuthorFaculty})");

                            if (hasAuthorFilter && !matchAuthor)
                            {
                                if (tempAuthorName.Contains(criteria.Author, StringComparison.OrdinalIgnoreCase))
                                    matchAuthor = true;
                            }
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.Name == "book")
                    {
                        if (matchGenre && matchAuthor && matchFaculty && currentBook != null)
                        {
                            results.Add(currentBook);
                        }
                        currentBook = null;
                    }
                    currentElement = "";
                }
            }
        }
        return results;
    }
}