using CsvHelper.Configuration;

namespace GRA.Domain.Service.Maps
{
    public class BranchMap : ClassMap<Model.Branch>
    {
        public BranchMap()
        {
            int index = 0;
            Map(_ => _.Id).Index(index++).Name(nameof(Model.Branch.Id));
            Map(_ => _.SystemName).Index(index++).Name(nameof(Model.Branch.SystemName));
            Map(_ => _.Name).Index(index++).Name(nameof(Model.Branch.Name));
            Map(_ => _.Telephone).Index(index++).Name(nameof(Model.Branch.Telephone));
            Map(_ => _.Address).Index(index++).Name(nameof(Model.Branch.Address));
            Map(_ => _.Url).Index(index).Name(nameof(Model.Branch.Url));
        }
    }
}
