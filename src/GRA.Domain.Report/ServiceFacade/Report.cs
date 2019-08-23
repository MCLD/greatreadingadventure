using System;
using GRA.Abstract;
using GRA.Domain.Repository;
using Microsoft.Extensions.Configuration;

namespace GRA.Domain.Report.ServiceFacade
{
    public class Report
    {
        public IConfiguration Config { get; }
        public IDateTimeProvider DateTimeProvider { get; }
        public IReportCriterionRepository ReportCriterionRepository { get; }
        public IReportRequestRepository ReportRequestRepository { get; }

        public ISiteSettingRepository SiteSettingRepository { get; }

        public Report(IConfiguration config,
            IDateTimeProvider dateTimeProvider,
            IReportCriterionRepository reportCriterionRepository,
            IReportRequestRepository reportRequestRepository,
            ISiteSettingRepository siteSettingRepository)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            DateTimeProvider = dateTimeProvider
                ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            ReportCriterionRepository = reportCriterionRepository
                ?? throw new ArgumentNullException(nameof(reportCriterionRepository));
            ReportRequestRepository = reportRequestRepository
                ?? throw new ArgumentNullException(nameof(reportRequestRepository));
            SiteSettingRepository = siteSettingRepository
                ?? throw new ArgumentNullException(nameof(siteSettingRepository));
        }
    }
}
