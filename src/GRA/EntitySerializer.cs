using Newtonsoft.Json;

namespace GRA
{
    public class EntitySerializer : Abstract.IEntitySerializer
    {
        public string Serialize(object entity)
        {
            return JsonConvert.SerializeObject(entity,
                Formatting.None,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
        }
    }
}