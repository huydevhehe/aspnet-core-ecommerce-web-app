/*namespace lab1.Extensions
{
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    public static class SessionExtensions
    {
        public static void SetJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T? GetJson<T>(this ISession session, string key) where T : class
        {
            var sessionData = session.GetString(key);
            return sessionData == null ? null : JsonConvert.DeserializeObject<T>(sessionData);
        }
    }
}
*/