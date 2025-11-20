using System.Xml;
using System.Xml.Xsl;

namespace Lab2OOP;

public class HtmlTransformer
{
    public void Transform(Stream xmlStream, Stream xslStream, string outputHtmlPath)
    {
        XslCompiledTransform xslt = new XslCompiledTransform();

        xslt.Load(XmlReader.Create(xslStream));

        using (var outputStream = new FileStream(outputHtmlPath, FileMode.Create))
        {
            xslt.Transform(XmlReader.Create(xmlStream), null, outputStream);
        }
    }
}