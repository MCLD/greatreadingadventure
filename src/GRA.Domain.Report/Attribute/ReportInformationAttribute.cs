using System;
using System.Collections.Generic;
using System.Text;

namespace GRA.Domain.Report.Attribute
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ReportInformationAttribute : System.Attribute
    {
        private readonly int _id;
        private readonly string _name;
        private readonly string _description;
        private readonly string _category;
        public ReportInformationAttribute(int id, string name, string description, string category)
        {
            _id = id;
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            _name = name;
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description));
            }
            _description = description;
            _category = category;
        }

        public int Id { get { return _id; } }
        public string Name { get { return _name; } }
        public string Description { get { return _description; } }
        public string Category { get { return _category; } }
    }
}
