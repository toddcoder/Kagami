using Core.Computers;
using Core.Configurations;
using Core.Json;
using Core.Monads;
using Core.WinForms;
using Core.WinForms.Controls;

namespace Kagami.GuiTests;

public partial class Tests : Form
{
   protected UiAction uiMessage = new();
   protected UiAction uiRun = new();
   protected UiAction uiGenerate = new();

   public Tests()
   {
      InitializeComponent();

      uiMessage.NoStatus("");

      uiRun.Button("Run All");
      uiRun.KeyDownCaption = new KeyDownCapture.ControlKey("Run Selected");
   }

   protected Optional<FolderName> getFolder()
   {
      FileName configurationFile = @"~\AppData\Local\Kagami.GuiTests\configuration.json";
      if (configurationFile)
      {
         return
            from setting in Deserializer.Deserialize(configurationFile).Optional()
            from folderName in setting.Result.FolderName("testFolder")
            select folderName;
      }
      else
      {
         var _folder = StandardDialog.BrowseFolder(this, "Test Folder", (FolderName)@"~\AppData\Local");
         if (_folder is (true, var folder))
         {
            var setting = new Setting();
            setting.Set("testFolder").FolderName = folder;

            return Serializer.Serialize(configurationFile, setting).Map(_ => folder).Optional();
         }
         else
         {
            return _folder.Exception;
         }
      }
   }
}