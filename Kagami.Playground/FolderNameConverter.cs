using System;
using Core.Computers;
using Newtonsoft.Json;

namespace Kagami.Playground
{
	public class FolderNameConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue(value.ToString());
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var folderName = (FolderName)reader.Value.ToString();
			return folderName;
		}

		public override bool CanConvert(Type objectType) => typeof(FolderName) == objectType;
	}
}