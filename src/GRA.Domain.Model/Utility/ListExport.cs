using System;
using System.Collections.Generic;

namespace GRA.Domain.Model.Utility
{
    public class ListExport<T> : Abstract.BaseImportExport
    {
        public IEnumerable<T> Data { get; set; }
    }
}
