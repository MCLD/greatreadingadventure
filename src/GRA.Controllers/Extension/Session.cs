using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace GRA.Controllers.Extension
{
    public static class SessionExtension
    {
        // Json session conversion code by Ben Cull
        // http://benjii.me/2016/07/using-sessions-and-httpcontext-in-aspnetcore-and-mvc-core/
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
