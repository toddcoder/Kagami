using Core.Computers;

namespace Kagami.Playground;

public class PlaygroundConfiguration
{
   public FileName LastFile { get; set; } = @"tutorial.kagami";

   public FolderName DefaultFolder { get; set; } = @"C:\";

   public string FontName { get; set; } = "Consolas";

   public float FontSize { get; set; } = 12;

   public FolderName PackageFolder { get; set; } = @"C:\";
}