using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2OOP;
public class BookResult
{
    public int Number { get; set; }
    public string Title { get; set; }
    public string Genre { get; set; }
    public string Year { get; set; }
    public List<string> AuthorsInfo { get; set; } = new List<string>();
    public string AuthorsDisplay => string.Join(", ", AuthorsInfo);
}
