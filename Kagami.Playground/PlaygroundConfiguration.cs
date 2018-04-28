using Standard.Computer;

namespace Kagami.Playground
{
   public class PlaygroundConfiguration
   {
      public FileName LastFile { get; set; }

      public FolderName DefaultFolder { get; set; }

      public string FontName { get; set; }

      public float FontSize { get; set; }

      public FolderName PackageFolder { get; set; }
   }
}