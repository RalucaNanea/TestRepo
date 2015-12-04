using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NUnitAutomationFramework
{
    public class ObjectMap
    {
       // public static string jsonExternalFile = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, "JsonFile.json");
        public static string jsonExternalFile = Utils.combineDirectoryPathWith(@"Helpers/JsonFile.json");
      
        //map JSON (deserialize) to custom .NET class for further usage
        public static List<T> _download_serialized_json_data<T>(string url) where T : new()
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    json_data = w.DownloadString(url);
                }
                catch (Exception) { }
                // if string with JSON data is not empty, deserialize it to class and return its instance 
                return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<List<T>>(json_data) : new List<T>();
            }
        }


        public static List<Selectors> mapJSONtoClass()
        {
            List<Selectors> selectors = _download_serialized_json_data<Selectors>(jsonExternalFile);
            return selectors;
        }

    }
}
