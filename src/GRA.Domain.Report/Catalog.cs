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

            foreach (var reportType in GetReports())
            {
                var attr = reportType.GetCustomAttribute<ReportInformationAttribute>();
                if (attr != null)
                {
                    reports.Add(new ReportDetails
                    {
                        Category = attr.Category,
                        Description = attr.Description,
                        Id = attr.Id,
                        Name = attr.Name,
                        ReportType = reportType.AsType(),
                        RequiresPermission = attr.RequiresPermission
                    });
                }
            }
            return reports;
        }

        public ReportDetails Get(int reportId)
        {
            foreach (var reportType in GetReports())
            {
                var attr = reportType.GetCustomAttribute<ReportInformationAttribute>();
                if (attr?.Id == reportId)
                {
                    return new ReportDetails
                    {
                        Category = attr.Category,
                        Description = attr.Description,
                        Id = attr.Id,
                        Name = attr.Name,
                        ReportType = reportType.AsType(),
                        RequiresPermission = attr.RequiresPermission
                    };
                }
            }
            return null;
        }

        private IEnumerable<TypeInfo> GetReports()
        {
            return GetType().GetTypeInfo().Assembly.DefinedTypes
                .Where(_ => _.IsClass
                    && !_.IsAbstract
                    && _.IsSubclassOf(typeof(Abstract.BaseReport)));
        }
    }
}
