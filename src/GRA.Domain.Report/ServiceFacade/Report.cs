using System;
using GRA.Abstract;
using GRA.Domain.Repository;
using Microsoft.Extensions.Configuration;

namespace GRA.Domain.Report.ServiceFacade
{
    public class Report
    {
        private readonly IConfigurationRoot _config;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IReportCriterionRepository _reportCriterionRepository;
        private readonly IReportRequestRepository _reportRequestRepository;
        private readonly ISiteSettingRepository _siteSettingRepository;

        public IConfigurationRoot Config
        {
            get
            {
                return _config;
            }
        }
        public IDateTimeProvider DateTimeProvider
        {
            get
            {
                return _dateTimeProvider;
            }
        }
        public IReportCriterionRepository ReportCriterionRepository
        {
            get
            {
                return _reportCriterionRepository;
            }
        }
        public IReportRequestRepository ReportRequestRepository
        {
            get
            {
                return _reportRequestRepository;
            }
        }

        public ISiteSettingRepository SiteSettingRepository
        {
            get
            {
                return _siteSettingRepository;
            }
        }


        public Report(IConfigurationRoot config,
            IDateTimeProvider dateTimeProvider,
            IReportCriterionRepository reportCriterionRepository,
            IReportRequestRepository reportRequestRepository,
            ISiteSettingRepository siteSettingRepository)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _dateTimeProvider = dateTimeProvider
                ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            _reportCriterionRepository = reportCriterionRepository
                ?? throw new ArgumentNullException(nameof(reportCriterionRepository));
            _reportRequestRepository = reportRequestRepository
                ?? throw new ArgumentNullException(nameof(reportRequestRepository));
            _siteSettingRepository = siteSettingRepository
                ?? throw new ArgumentNullException(nameof(SiteSettingRepository));
        }
    }
}
