using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GRA.Domain.Model;
using GRA.Utility;
using Xunit;

namespace GRA.Test
{
    public class VerifyResourcesTest
    {
        private const string BasePathToResx = "..{0}..{0}..{0}..{0}..{0}src{0}GRA{0}Resources";
        private const char NonBreakingSpaceCharacter = (char)160;
        private const string ResxFileEnglish = "Shared.en.resx";
        private const string ResxFileSpanish = "Shared.es.resx";
        private const char SpaceCharacter = (char)32;
        private readonly string PathToResx;

        public VerifyResourcesTest()
        {
            PathToResx = Path.GetFullPath(
                Path.Combine(AppContext.BaseDirectory,
                string.Format(BasePathToResx, Path.DirectorySeparatorChar)));
        }

        [Fact]
        public void CheckUntranslatedItemsEnglish() => CheckUntranslatedItemsFile(ResxFileEnglish);

        [Fact]
        public void CheckUntranslatedItemsSpanish() => CheckUntranslatedItemsFile(ResxFileSpanish);

        [Fact]
        public void TestEnglishResxHasAllItems() => TestResxHasItems(ResxFileEnglish);

        [Fact]
        public void TestSpanishResxHasAllItems() => TestResxHasItems(ResxFileSpanish);

        private void CheckUntranslatedItemsFile(string filename)
        {
            Assert.Empty(XmlParser.ExtractDataNames(filename, PathToResx)
                .Where(_ => _.Value == "{0}")
                .Select(_ => _.Key.Replace(NonBreakingSpaceCharacter, SpaceCharacter)));
        }

        private void TestResxHasItems(string filename)
        {
            var resourceFileKeys = XmlParser.ExtractDataNames(filename, PathToResx)
                .Where(_ => !string.IsNullOrEmpty(_.Value))
                .Select(_ => _.Key.Replace(NonBreakingSpaceCharacter, SpaceCharacter));

            var items = new List<string>();
            foreach (var classType in typeof(Annotations).GetNestedTypes(BindingFlags.Public))
            {
                var constStrings = classType.GetFields(BindingFlags.Public
                    | BindingFlags.Static
                    | BindingFlags.FlattenHierarchy)
                    .Where(_ => _.IsLiteral && !_.IsInitOnly)
                    .ToList();

                foreach (var constval in constStrings)
                {
                    items.Add((string)constval.GetValue(null));
                }
            }

            var displayNameConstStrings = typeof(DisplayNames).GetFields(BindingFlags.Public
                    | BindingFlags.Static
                    | BindingFlags.FlattenHierarchy)
                    .Where(_ => _.IsLiteral && !_.IsInitOnly)
                    .ToList();
            foreach (var constval in displayNameConstStrings)
            {
                items.Add((string)constval.GetValue(null));
            }

            var errorMessageConstStrings = typeof(ErrorMessages).GetFields(BindingFlags.Public
                    | BindingFlags.Static
                    | BindingFlags.FlattenHierarchy)
                    .Where(_ => _.IsLiteral && !_.IsInitOnly)
                    .ToList();
            foreach (var constval in errorMessageConstStrings)
            {
                items.Add((string)constval.GetValue(null));
            }

            Assert.Empty(items.Except(resourceFileKeys));
        }
    }
}
