using Core.Computers;
using Newtonsoft.Json;

namespace Kagami.Playground
{
   public class PlaygroundConfiguration
   {
		[JsonConverter(typeof(FileNameConverter))]
      public FileName LastFile { get; set; }

      [JsonConverter(typeof(FolderNameConverter))]
      public FolderName DefaultFolder { get; set; }

      public string FontName { get; set; }

      public float FontSize { get; set; }

      [JsonConverter(typeof(FolderNameConverter))]
      public FolderName PackageFolder { get; set; }
   }
}