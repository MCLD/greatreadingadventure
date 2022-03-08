using System;
using System.Collections.Generic;

namespace GRA.Domain.Model.Utility
{
    public class DirectEmailDetails
    {
        private readonly string _siteName;

        public DirectEmailDetails(string siteName)
        {
            if (string.IsNullOrEmpty(siteName))
            {
                throw new ArgumentNullException(nameof(siteName));
            }

            _siteName = siteName;
            InternalTags = new Dictionary<string, string>();
            ClearTags();
        }

        /// <summary>
        /// A DirectEmailSystemId value to use for this email template if it's a system-generated
        /// email. Ignored if DirectEmailTemplateId is set.
        /// </summary>
        public string DirectEmailSystemId { get; set; }

        /// <summary>
        /// The DirectEmailTemplateId to use for this email.
        /// </summary>
        public int DirectEmailTemplateId { get; set; }

        public bool IsBulk { get; set; }

        /// <summary>
        /// True if this is a test email and shouldn't be counted towards the total number of
        /// emails sent.
        /// </summary>
        public bool IsTest { get; set; }

        /// <summary>
        /// Lanaguage ID of the template to use, ignored if ToUserId is defined.
        /// </summary>
        public int? LanguageId { get; set; }

        public int SendingUserId { get; set; }

        public IDictionary<string, string> Tags
        {
            get
            {
                return InternalTags;
            }
        }

        /// <summary>
        /// Email address for who the email is sent to, ignored if ToUserId is defined.
        /// </summary>
        public string ToAddress { get; set; }

        /// <summary>
        /// Name for who the email is sent to, ignored if ToUserId is defined.
        /// </summary>
        public string ToName { get; set; }

        /// <summary>
        /// The User to send the email to. If this is configured then ToAddress, ToName, and
        /// LanguageId are ignored.
        /// </summary>
        public int? ToUserId { get; set; }

        private IDictionary<string, string> InternalTags { get; }

        public void ClearTags()
        {
            InternalTags.Clear();
            InternalTags.Add("Sitename", _siteName);
        }

        public void SetTag(string key, string value)
        {
            if (InternalTags.ContainsKey(key))
            {
                InternalTags[key] = value;
            }
            else
            {
                InternalTags.Add(key, value);
            }
        }
    }
}
