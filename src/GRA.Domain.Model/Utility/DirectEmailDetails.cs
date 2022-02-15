using System;
using System.Collections.Generic;

namespace GRA.Domain.Model.Utility
{
    public class DirectEmailDetails
    {
        public DirectEmailDetails(string siteName)
        {
            if (string.IsNullOrEmpty(siteName))
            {
                throw new ArgumentNullException(nameof(siteName));
            }

            Tags = new Dictionary<string, string>
            {
                { "Sitename", siteName }
            };
        }

        public string DirectEmailSystemId { get; set; }
        public int DirectEmailTemplateId { get; set; }
        public IDictionary<string, string> Tags { get; }
    }
}
