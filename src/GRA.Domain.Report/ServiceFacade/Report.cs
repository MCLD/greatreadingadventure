using System;
using GRA.Abstract;
using GRA.Domain.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace GRA.Domain.Report.ServiceFacade
{
    public class Report
    {
        public Report(IConfiguration config,
            IDateTimeProvider dateTimeProvider,
            IReportCriterionRepository reportCriterionRepository,
            IReportRequestRepository reportRequestRepository,
            ISiteSettingRepository siteSettingRepository)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(dateTimeProvider);
            ArgumentNullException.ThrowIfNull(reportCriterionRepository);
            ArgumentNullException.ThrowIfNull(reportRequestRepository);
            ArgumentNullException.ThrowIfNull(siteSettingRepository);

            Config = config;
            DateTimeProvider = dateTimeProvider;
            ReportCriterionRepository = reportCriterionRepository;
            ReportRequestRepository = reportRequestRepository;
            SiteSettingRepository = siteSettingRepository;
        }

        public IConfiguration Config { get; }
        public IDateTimeProvider DateTimeProvider { get; }
        public IHttpContextAccessor HttpContextAccessor { get; }
        public IReportCriterionRepository ReportCriterionRepository { get; }
        public IReportRequestRepository ReportRequestRepository { get; }

        public ISiteSettingRepository SiteSettingRepository { get; }
    }
}
