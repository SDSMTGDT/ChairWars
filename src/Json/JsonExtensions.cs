using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;


namespace ChairWars.Json
{
    public static class JsonExtensions
    {
        public static Newtonsoft.Json.Formatting Formatting;
        public static Newtonsoft.Json.JsonSerializer serializer;
        public static JsonSerializerSettings settings;

        static JsonExtensions()
        {
            
            Formatting = Newtonsoft.Json.Formatting.Indented;
            settings = new JsonSerializerSettings();
            settings.Converters.Add(new GeneralJsonEnumConverter());
            serializer = new JsonSerializer();
           // serializer.NullValueHandling = NullValueHandling.Ignore;
           // serializer.ObjectCreationHandling = ObjectCreationHandling.Auto;
            //serializer.TypeNameHandling = TypeNameHandling.Objects;
           // serializer.MissingMemberHandling = MissingMemberHandling.Ignore;
           // serializer.DefaultValueHandling = DefaultValueHandling.Include;

            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.ObjectCreationHandling = ObjectCreationHandling.Auto;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            settings.TypeNameHandling = TypeNameHandling.Auto;
            settings.MissingMemberHandling = MissingMemberHandling.Ignore;
            settings.DefaultValueHandling = DefaultValueHandling.Include;
            
        }

        public static string ToJson<T>(ref T obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting, settings);
        }

        // Switch to serializer for more control? Perhaps...
        public static void ToJsonFile<T>(string fileName, ref T obj)
        {
            using (var stream = new StreamWriter(fileName))
            using (var writer = new JsonTextWriter(stream))
            {
                writer.WriteRawValue(ToJson(ref obj));
            }
        }

        public static bool FromJson<T>(string rawJson, ref IInitialize obj) where T : IInitialize
        {
            obj = default(T);
            obj = JsonConvert.DeserializeObject<T>(rawJson, settings);
            if (obj != null)
            {
                obj.Initialize();
                return true;
            }

            return false;
        }

        public static bool FromJson<T>(string rawJson, ref T obj)
        {
            obj = default(T);
            obj = JsonConvert.DeserializeObject<T>(rawJson, settings);
            if (obj != null)
            {
                return true;
            }

            return false;
        }

        private static T FromJsonFileHelper<T>(string fileName, ref T obj)
        {
            obj = default(T);
            using (var stream = new StreamReader(fileName))
            using (var reader = new JsonTextReader(stream))
            {
                //obj = JsonConvert.DeserializeObject<T>(stream.ReadToEnd(), settings);
                return serializer.Deserialize<T>(reader);
            }
        }

        private static string FromJsonFileHelper(string fileName)
        {
            using (var stream = new StreamReader(fileName))
            //using (var reader = new JsonTextReader(stream))
            {
                //obj = JsonConvert.DeserializeObject<T>(stream.ReadToEnd(), settings);
                //return serializer.Deserialize<T>(reader);
                return stream.ReadToEnd();
            }
        }

        public static bool PopulateObjectFromJsonFile<T>(string fileName, ref T obj)
        {
            if (obj == null)
            {
                return false;
            }
            JsonConvert.PopulateObject(FromJsonFileHelper(fileName), obj);

            return true;
        }

        public static bool FromJsonFileAndInit<T>(string fileName, ref T obj) where T : IInitialize
        {
            obj = FromJsonFileHelper(fileName, ref obj);
            if (obj != null)
            {
                obj.Initialize();
                return true;
            }

            return false;
        }

        public static bool FromJsonFile<T>(string fileName, ref T obj)
        {
            obj = FromJsonFileHelper(fileName, ref obj);
            if (obj != null)
            {
                return true;
            }

            return false;
        }
    }

    public class GeneralJsonEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsEnum;
        }

        public override void WriteJson(JsonWriter writer, object
        value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type
        objectType, object existingValue, JsonSerializer serializer)
        {
            return Enum.Parse(objectType, reader.Value.ToString());
        }
    }

    public interface IInitialize
    {
        void Initialize();
    }
}
