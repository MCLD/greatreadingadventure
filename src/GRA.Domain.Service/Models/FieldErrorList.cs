using System.Collections.Generic;
using System.Linq;

namespace GRA.Domain.Service.Models
{
    public class FieldErrorList
    {
        private readonly IList<KeyValuePair<string, string>> errorList;

        public FieldErrorList()
        {
            errorList = new List<KeyValuePair<string, string>>();
        }

        public void Add(string fieldName, string errorMessage)
        {
            errorList.Add(new KeyValuePair<string, string>(fieldName, errorMessage));
        }

        public ILookup<string, string> AsILookup()
        {
            return errorList.ToLookup(_ => _.Key, _ => _.Value);
        }
    }
}
