using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace GRA.Utility
{
    public static class XmlParser
    {
        public static Dictionary<string, string> ExtractDataNames(string filename, string path)
        {
            var finalDictionary = new Dictionary<string, string>();
            var xmlFilepath = Path.Combine(path, filename);
            foreach (var element in XElement.Load(xmlFilepath).Descendants("data"))
            {
                finalDictionary[element.Attribute("name").Value] = element
                    .Descendants("value")
                    .FirstOrDefault()?
                    .Value;
            }
            return finalDictionary;
        }
    }
}
