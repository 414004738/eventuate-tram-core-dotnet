using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IO.Eventuate.Tram
{
	public static class JsonMapper
	{
		public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver
			{
				// Prevent Header dictionary keys from being camel cased
				NamingStrategy = {ProcessDictionaryKeys = false}
			},
			MissingMemberHandling = MissingMemberHandling.Ignore,
			NullValueHandling = NullValueHandling.Ignore
		};
		
		public static string ToJson(object o)
		{
			return JsonConvert.SerializeObject(o, JsonSerializerSettings);
		}

		public static T FromJson<T>(string json)
		{
			return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);
		}
		
		public static object FromJson(string json, Type type)
		{
			return JsonConvert.DeserializeObject(json, type, JsonSerializerSettings);
		}
	}
}