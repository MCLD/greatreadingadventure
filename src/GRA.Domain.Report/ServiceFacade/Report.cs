using System;
using GRA.Abstract;
using GRA.Domain.Repository;

namespace GRA.Domain.Report.ServiceFacade
{
    public class Report
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IReportCriterionRepository _reportCriterionRepository;
        private readonly IReportRequestRepository _reportRequestRepository;

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


        public Report(IDateTimeProvider dateTimeProvider,
            IReportCriterionRepository reportCriterionRepository,
            IReportRequestRepository reportRequestRepository)
        {
            _dateTimeProvider = dateTimeProvider
                ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            _reportCriterionRepository = reportCriterionRepository
                ?? throw new ArgumentNullException(nameof(reportCriterionRepository));
            _reportRequestRepository = reportRequestRepository
                ?? throw new ArgumentNullException(nameof(reportRequestRepository));
        }
    }
}
