using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using Xunit;
using System.Linq;

namespace GRA.Test
{
    public class ReflectionTest
    {
        private readonly IStringLocalizer<ReflectionTest> _localizer;

        public ReflectionTest()
        {
            Console.WriteLine("Starting Tests");
        }

        [Fact]
        public void Test1()
        {
            ResourceManager rm = new ResourceManager("Shared.en.resx", typeof(GRA.Resources.Shared).Assembly);
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var resourceName in assembly.GetManifestResourceNames())
                System.Console.WriteLine("YOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO" + resourceName);
           // var resourceNames = _localizer.GetAllStrings().Select(x => x.Name);
            //var resourceSet = _localizer.WithCulture(new CultureInfo("en")).GetAllStrings().Select(x => x.Name);
            String strWebsite = rm.GetString("All Programs", CultureInfo.CurrentCulture);
            String strName = rm.GetString("Name");

            Assert.True(true);
        }
    }
}
