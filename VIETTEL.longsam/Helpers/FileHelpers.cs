using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace VIETTEL.Helpers
{
    public static class FileHelpers
    {
        #region sql

        public static string GetSqlQueryFile(string filename)
        {
            var filePath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/sql/" + filename);
            return System.IO.File.ReadAllText(filePath);
        }

        public static string GetSqlQuery(string filename, bool noEmpty = true)
        {
            var path = getSqlFile(filename);

            var declareSeparator = "--#DECLARE#--";
            var builder = new StringBuilder();
            foreach (string line in File.ReadLines(path))
            {
                if (line.Contains(declareSeparator) || line.Contains("--###--"))
                {
                    builder.Clear();
                }
                else
                {
                    if (!noEmpty || !string.IsNullOrWhiteSpace(line))
                        builder.AppendLine(line);
                }
            }

            return builder.ToString();
        }

        private static string getSqlFile(string filename)
        {
            var path = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/sql/" + filename);

            if (File.Exists(path))
            {
                return path;
            }

            try
            {
                var folder = System.Web.HttpContext.Current.Server.MapPath(@"~/App_Data/sql");
                foreach (string file in Directory.GetFiles(folder, "*.sql*", SearchOption.AllDirectories))
                {
                    if (file.ToLower().Contains($"\\{filename.ToLower()}"))
                    {
                        return file;
                    }
                }
            }
            catch (System.Exception e)
            {
                //Console.WriteLine(e.Message);
            }

            return path;
        }

        public static string GetSqlQueryPath(string filePath, bool noEmpty = true)
        {
            var path = System.Web.HttpContext.Current.Server.MapPath(filePath);

            var declareSeparator = "--#DECLARE#--";
            var builder = new StringBuilder();
            foreach (string line in File.ReadLines(path))
            {
                if (line.Contains(declareSeparator) || line.Contains("--###--"))
                {
                    builder.Clear();
                }
                else
                {
                    if (!noEmpty || !string.IsNullOrWhiteSpace(line))
                        builder.AppendLine(line);
                }
            }

            return builder.ToString();
        }


        #endregion sql

        #region xml
        public static T ReadSetting<T>(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open);
                return (T)serializer.Deserialize(fs);
            }
            catch
            {
                return default(T);
            }
        }
        #endregion
    }
}
