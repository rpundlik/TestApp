using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace LMS.Web.Core
{
    public static class LoggedIn
    {
       

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

    public class SessionServices
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public SessionServices(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetSession(string key, object value)
        {

            _session.SetObjectAsJson(key, value);
        }

        public T GetSession<T>(string key)
        {
            return _session.GetObjectFromJson<T>(key);
            
        }
    }
}
