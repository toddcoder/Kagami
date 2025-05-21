using Core.Computers;
using Core.Configurations;
using Core.Json;
using Core.Monads;
using static Core.Json.Deserializer;

namespace Kagami.Playground;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public class PlaygroundConfiguration
{
   protected const string CONFIGURATION_FOLDER = @"~\AppData\Local\Kagami";
   protected const string DEFAULT_FOLDER = @"~\Documents\Kagami";

   public static Result<PlaygroundConfiguration> Retrieve()
   {
      try
      {
         FolderName folder = CONFIGURATION_FOLDER;
         var file = folder.Guarantee() + "playground.json";
         if (file)
         {
            var _setting = Deserialize(file);
            if (_setting is (true, var setting))
            {
               var configuration = new PlaygroundConfiguration
               {
                  DefaultFolder = setting.Maybe.FolderName("defaultFolder") | DEFAULT_FOLDER,
                  FontName = setting.Maybe.String("fontName") | "Consolas",
                  FontSize = setting.Maybe.Single("fontSize") | 12f,
                  PackageFolder = setting.Maybe.FolderName("packageFolder") | CONFIGURATION_FOLDER
               };
               configuration.LastFile = setting.Maybe.FileName("lastFile") | (() => configuration.DefaultFolder + "test.kagami");

               return configuration;
            }
            else
            {
               return new PlaygroundConfiguration();
            }
         }
         else
         {
            return new PlaygroundConfiguration();
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Result<Unit> Save()
   {
      try
      {
         var setting = new Setting();
         setting.Set("lastFile").FileName = LastFile;
         setting.Set("defaultFolder").FolderName = DefaultFolder;
         setting.Set("fontName").String = FontName;
         setting.Set("fontSize").Single = FontSize;
         setting.Set("packageFolder").FolderName = PackageFolder;
         FolderName configurationFolder = CONFIGURATION_FOLDER;
         var file = configurationFolder.Guarantee() + "playground.json";

         return Serializer.Serialize(file, setting).Unit;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public FileName LastFile { get; set; } = ((FolderName)DEFAULT_FOLDER).Guarantee() + @"test.kagami";

   public FolderName DefaultFolder { get; set; } = DEFAULT_FOLDER;

   public string FontName { get; set; } = "Consolas";

   public float FontSize { get; set; } = 12;

   public FolderName PackageFolder { get; set; } = CONFIGURATION_FOLDER;
}