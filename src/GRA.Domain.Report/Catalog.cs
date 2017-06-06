using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GRA.Domain.Model;
using GRA.Domain.Report.Attribute;

namespace GRA.Domain.Report
{
    public class Catalog
    {
        public IEnumerable<ReportDetails> Get()
        {
            var reports = new List<ReportDetails>();

            var reportTypes = GetType().GetTypeInfo().Assembly.DefinedTypes
                .Where(_ => _.IsClass
                    && !_.IsAbstract
                    && _.IsSubclassOf(typeof(Abstract.BaseReport)));

            foreach (var reportType in reportTypes)
            {
                var attr = reportType.GetCustomAttribute<ReportInformationAttribute>();
                if (attr != null)
                {
                    reports.Add(new ReportDetails
                    {
                        Id = attr.Id,
                        Name = attr.Name,
                        Description = attr.Description,
                        ReportType = reportType.AsType()
                    });
                }
            }
            return reports;
        }
    }
}
