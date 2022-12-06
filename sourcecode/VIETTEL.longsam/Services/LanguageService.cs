using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Viettel.Services
{
    public interface ILocalizationService
    {
        string Translate(string key);
        string Translate(string format, params object[] args);
    }
    public class LocalizationService : ILocalizationService
    {
        private readonly Dictionary<string, string> _data;

        public LocalizationService()
        {
            _data = getLanguages();
        }

        public string Translate(string key)
        {
            key = key.ToLower();

            if (_data.ContainsKey(key))
            {
                return _data[key];
            }
            else
            {
                return string.Format("[{0}]", key);
            }
        }

        public string Translate(string format, params object[] args)
        {
            //if (_data.ContainsKey(key))
            //{
            //    return _data[key];
            //}
            //else
            //{
            //    return string.Format("[{0}]", key);
            //}

            return format;
        }

        private static ILocalizationService _default;
        public static ILocalizationService Default
        {
            get { return _default ?? (_default = new LocalizationService()); }
        }

        private Dictionary<string, string> getLanguages()
        {
            var dic = new Dictionary<string, string>()
            {
               

                // B2
               

                {"Donvi_Cap1", "Bộ Quốc phòng" },
                {"Donvi_Cap2", "Cục Tài chính" },
                {"Donvi_Cap3", "Donvi_Cap3" },
            };

            dic = dic.ToDictionary(x => x.Key.ToLower(), x => x.Value);

            var dicFile = getLanguage("~/app_data/language/vi-vn.json");

            foreach (var item in dicFile)
            {
                if (dic.ContainsKey(item.Key))
                {
                    dic.Remove(item.Key.ToLower());

                }

                dic.Add(item.Key.ToLower(), item.Value);
            }

            // khong phan biet hoa thuong
            return dic;
        }

        private Dictionary<string, string> getLanguage(string file)
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath(file);

            var json = File.ReadAllText(path);
            var dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return dic;
        }



    }
}
