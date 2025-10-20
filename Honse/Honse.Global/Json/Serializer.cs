
namespace Honse.Global.Json
{
    public class Serializer
    {
        public static string ToJson(object obj, SerializationSettings settings = null)
        {
            settings = settings ?? new SerializationSettings();

            Newtonsoft.Json.JsonSerializerSettings newtonSoftSettings = new Newtonsoft.Json.JsonSerializerSettings();

            if (!string.IsNullOrEmpty(settings.DateFormatString))
                newtonSoftSettings.DateFormatString = settings.DateFormatString;
            if (settings.CamelCasePropertyNames)
                newtonSoftSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            if (settings.Indented)
                newtonSoftSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            if (settings.IgnoreNull)
                newtonSoftSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;

            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, newtonSoftSettings);
        }
        
        public static T ToObject<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        public static object ToObject(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(json);
        }
    }

    public class SerializationSettings
    {
        public bool CamelCasePropertyNames { get; set; }

        public bool Indented { get; set; }

        public bool IgnoreNull { get; set; }

        public bool AddNullToEmptyArrayConverter { get; set; }

        public string DateFormatString { get; set; } = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffff'Z'";
    }
}
