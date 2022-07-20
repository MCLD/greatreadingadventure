using System.Collections.Generic;

namespace GRA.Domain.Model.Utility
{
    public class ListImport<T> : Abstract.BaseImportExport
    {
        public IEnumerable<T> Data { get; set; }
    }
}
