using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Station
{
	public class JsonSerializer : ISerializer
	{
		public string Serialize<T>(T data)
		{
			string serializedData = null;
			try
			{
				serializedData = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Auto,
					ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
				});
			}
			catch (Exception e)
			{
				Debug.LogError($"An error happened while serializing the object : {e.Message}\n{e.StackTrace}");
			}

			return serializedData;
		}

		public T UnSerialize<T>(string serializedData)  
		{
			T unserializedData = default(T);

			try
			{
				unserializedData = JsonConvert.DeserializeObject<T>(serializedData, new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Auto,
					ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
				});
			}
			catch (Exception e)
			{
				Debug.LogError($"An error happened while serializing the object : {e.Message}\n{e.StackTrace}");
			}

			return unserializedData;
		}
	}
}