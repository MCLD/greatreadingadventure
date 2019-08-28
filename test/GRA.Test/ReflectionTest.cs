using System;
using System.Collections.Generic;
using System.Reflection;
using GRA.Utility;
using Microsoft.Extensions.Localization;
using Xunit;
using System.Linq;
using System.IO;

namespace GRA.Test
{
    public class ReflectionTest
    {
        public ReflectionTest()
        {
            Console.WriteLine("Starting Tests");
        }

        [Fact]
        public void TestEnglishResxHasAllAnnotations()
        {
            var separator = Path.DirectorySeparatorChar;
            string startupPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, $"..{separator}..{separator}..{separator}..{separator}..{separator}"));
            var dictionary = XmlParser.ParseSharedXml("Shared.en.resx", startupPath + $"src{separator}GRA{separator}Resources");
            var annotations = new List<string>();
            var errList = new List<string>();
            Type[] typeArray = typeof(Annotations).GetNestedTypes(BindingFlags.Public);
            foreach (var classType in typeArray)
            {
                var constStrings = classType.GetFields(BindingFlags.Public | BindingFlags.Static |
                    BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
                foreach (var constval in constStrings)
                {
                    annotations.Add((string)constval.GetValue(null));
                }
            }
            foreach(var str in annotations)
            {
                if (!dictionary.ContainsKey(str))
                {
                    errList.Add(str);
                }
            }
            if (errList.Count()>0)
            {
                throw new GraException($"Found {errList.Count()} not apart of english Resx:\n['{String.Join("' ,\n'", errList.ToArray())}']");
            }
            else
            {
                Assert.True(true);
            }
        }

        [Fact]
        public void TestSpanishResxHasAllAnnotations()
        {
            var separator = Path.DirectorySeparatorChar;
            string startupPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, $"..{separator}..{separator}..{separator}..{separator}..{separator}"));
            var dictionary = XmlParser.ParseSharedXml("Shared.es.resx", startupPath + $"src{separator}GRA{separator}Resources");
            var annotations = new List<string>();
            var errList = new List<string>();
            Type[] typeArray = typeof(Annotations).GetNestedTypes(BindingFlags.Public);
            foreach (var classType in typeArray)
            {
                var constStrings = classType.GetFields(BindingFlags.Public | BindingFlags.Static |
                    BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
                foreach (var constval in constStrings)
                {
                    annotations.Add((string)constval.GetValue(null));
                }
            }
            foreach (var str in annotations)
            {
                if (!dictionary.ContainsKey(str))
                {
                    errList.Add(str);
                }
            }
            if (errList.Count() > 0)
            {
                throw new GraException($"Found {errList.Count()} not apart of spanish Resx:\n['{String.Join("' ,\n'", errList.ToArray())}']");
            }
            else
            {
                Assert.True(true);
            }
        }
    }
}
