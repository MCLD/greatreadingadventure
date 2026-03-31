namespace GRA.Domain.Model
{
    public class SiteSettingDefinition
    {
        public string Category { get; set; }
        public string DefaultValue { get; set; }
        public SiteSettingFormat Format { get; set; }
        public string Info { get; set; }
        public string Name { get; set; }
    }
}
