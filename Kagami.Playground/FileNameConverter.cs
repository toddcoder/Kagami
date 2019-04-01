using System;
using Core.Computers;
using Newtonsoft.Json;

namespace Kagami.Playground
{
	public class FileNameConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue(value.ToString());
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var fileName = (FileName)reader.Value.ToString();
			return fileName;
		}

		public override bool CanConvert(Type objectType) => typeof(FileName) == objectType;
	}
}