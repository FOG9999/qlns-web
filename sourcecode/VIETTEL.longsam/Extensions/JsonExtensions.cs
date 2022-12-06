using Newtonsoft.Json;

namespace Viettel.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJson<T>(this T obj) where T : class
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T Clone<T>(this T source)
        {
            var json = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
