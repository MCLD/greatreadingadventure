using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GRA.Utility
{
    public class XmlParser
    {

        public static Dictionary<string,string> ParseSharedXml(string fileName, string directory)
        {
            var finalDictionary = new Dictionary<string, string>();
            var xmlFilepath = Path.Combine(directory, fileName);
            XElement xmlData = XElement.Load($"{xmlFilepath}");
            IEnumerable<XElement> partNos = from item in xmlData.Descendants("data") select item;
            foreach (var element in partNos)
            {
                finalDictionary[element.Attribute("name").Value] = element.Descendants("value")
                                                                        .FirstOrDefault()?.Value;
            }
            return finalDictionary;
        }
    }
}
