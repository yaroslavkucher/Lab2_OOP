using System.Xml;

namespace Lab2OOP;
public class SaxStrategy : IAnalysisStrategy
{
    public List<string> Search(SearchCriteria criteria, Stream xmlStream)
    {
        var results = new List<string>();
        using (XmlReader reader = XmlReader.Create(xmlStream))
        {
            string currentTitle = null;
            bool inBook = false;

            bool matchesGenre = true;
            bool foundMatchingAuthor = false;
            bool foundMatchingFaculty = false;

            bool authorSearchRequired = !string.IsNullOrWhiteSpace(criteria.Author);
            bool facultySearchRequired = !string.IsNullOrWhiteSpace(criteria.Faculty);
            bool genreSearchRequired = !string.IsNullOrWhiteSpace(criteria.Genre);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "book")
                    {
                        inBook = true;
                        currentTitle = null;

                        if (genreSearchRequired)
                        {
                            string genre = reader.GetAttribute("genre");
                            matchesGenre = (genre != null && genre.Equals(criteria.Genre, StringComparison.OrdinalIgnoreCase));
                        }
                        else
                        {
                            matchesGenre = true;
                        }

                        foundMatchingAuthor = !authorSearchRequired;
                        foundMatchingFaculty = !facultySearchRequired;
                    }
                    else if (inBook && reader.Name == "title")
                    {
                        reader.Read();
                        if (reader.NodeType == XmlNodeType.Text)
                            currentTitle = reader.Value;
                    }
                    else if (inBook && matchesGenre && reader.Name == "author")
                    {
                        if (facultySearchRequired && !foundMatchingFaculty)
                        {
                            string faculty = reader.GetAttribute("faculty");
                            if (faculty != null && faculty.Equals(criteria.Faculty, StringComparison.OrdinalIgnoreCase))
                            {
                                foundMatchingFaculty = true;
                            }
                        }

                        if (authorSearchRequired && !foundMatchingAuthor)
                        {
                            reader.Read();
                            if (reader.NodeType == XmlNodeType.Text && reader.Value != null && reader.Value.Contains(criteria.Author, StringComparison.OrdinalIgnoreCase))
                            {
                                foundMatchingAuthor = true;
                            }
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "book")
                {
                    if (matchesGenre && foundMatchingAuthor && foundMatchingFaculty && currentTitle != null)
                    {
                        results.Add(currentTitle);
                    }
                    inBook = false;
                }
            }
        }
        return results;
    }
}