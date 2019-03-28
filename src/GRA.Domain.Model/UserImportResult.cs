using System.Collections.Generic;

namespace GRA.Domain.Model
{
    public class UserImportResult
    {
        public List<UserImportExport> Users { get; set; }
        public List<string> Errors { get; set; }
    }
}
