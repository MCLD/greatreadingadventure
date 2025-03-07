using System;

namespace GRA.Domain.Report.Attribute
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ReportInformationAttribute : System.Attribute
    {
        private readonly string _category;
        private readonly string _description;
        private readonly int _id;
        private readonly string _name;
        private readonly string _requiresPermission;

        public ReportInformationAttribute(int id,
            string name,
            string description,
            string category,
            string requiresPermission = null)
        {
            _category = category;
            _description = description;
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
            _requiresPermission = requiresPermission;
        }

        public string Category
        { get { return _category; } }

        public string Description
        { get { return _description; } }

        public int Id
        { get { return _id; } }

        public string Name
        { get { return _name; } }

        public string RequiresPermission
        { get { return _requiresPermission; } }
    }
}
