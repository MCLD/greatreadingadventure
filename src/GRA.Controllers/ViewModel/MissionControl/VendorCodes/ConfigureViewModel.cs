using System;
using System.Collections.Generic;
using System.Linq;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.VendorCodes
{
    public class ConfigureViewModel
    {
        public IDictionary<int, string> DirectEmailTemplates { get; set; }
        public IDictionary<string, int> Languages { get; set; }
        public IDictionary<int, int[]> MessageTemplateLanguageIds { get; set; }
        public IDictionary<int, int[]> SegmentLanguageIds { get; set; }
        public VendorCodeType VendorCodeType { get; set; }

        public bool AnyConfiguredLanguages(int? messageTemplateId)
        {
            return messageTemplateId.HasValue
                && MessageTemplateLanguageIds.ContainsKey(messageTemplateId.Value);
        }

        public bool AnyConfiguredSegmentLanguages(int? segmentId)
        {
            return segmentId.HasValue
                && SegmentLanguageIds.ContainsKey(segmentId.Value);
        }

        public string LanguageMessageClass(int? messageTemplateId, int languageId)
        {
            return !messageTemplateId.HasValue
                ? "btn-secondary"
                : MessageTemplateLanguageIds.TryGetValue(messageTemplateId.Value, out int[] value)
                    && value.Contains(languageId)
                    ? "btn-success"
                    : "btn-warning";
        }

        public string LanguageSegmentClass(int? segmentId, int languageId)
        {
            return !segmentId.HasValue
                ? "btn-secondary"
                : SegmentLanguageIds.TryGetValue(segmentId.Value, out int[] value)
                    && value.Contains(languageId)
                    ? "btn-success"
                    : "btn-warning";
        }
    }
}
