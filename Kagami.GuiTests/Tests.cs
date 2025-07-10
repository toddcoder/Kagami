using System.Text;
using Core.Applications.Messaging;
using Core.Computers;
using Core.Configurations;
using Core.Json;
using Core.Monads;
using Core.WinForms;
using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;
using static Core.Monads.MonadFunctions;

namespace Kagami.GuiTests;

public partial class Tests : Form
{
   protected UiAction uiMessage = new();
   protected UiAction uiRun = new();
   protected UiAction uiGenerate = new();
   protected ExRichTextBox textSource = new();
   protected TestBackground testBackground;
   protected GenerateBackground generateBackground;

   public Tests()
   {
      InitializeComponent();

      testBackground = new TestBackground(nil, listViewTests)
      {
         FolderNotFound =
         {
            Handler = () => uiMessage.Failure("Test folder not found")
         },
         Finalized =
         {
            Handler = folder => uiMessage.Success($"Tests in {folder} completed")
         },
         Progress =
         {
            Handler = text => uiMessage.Do(() => uiMessage.Busy(text))
         }
      };
      generateBackground = new GenerateBackground(nil, listViewTests)
      {
         FolderNotFound =
         {
            Handler = () => uiMessage.Failure("Test folder not found")
         },
         Finalized =
         {
            Handler = folder => uiMessage.Success($"Expected texts in {folder} generated")
         },
         Progress =
         {
            Handler = text => uiMessage.Do(() => uiMessage.Busy(text))
         }
      };

      uiMessage.NoStatus("");

      uiRun.Button("Run All");
      uiRun.KeyDownCaption = new KeyDownCapture.ControlKey("Run Selected");
      uiRun.Click += (_, _) =>
      {
         var _folder = getFolder();
         if (_folder is (true, var testFolder))
         {
            if (uiRun.IsKeyDown)
            {
               if (listViewTests.SelectedItem() is (true, var item))
               {
                  var name = item.Text;
                  testBackground.File = testFolder + $"{name}.kagami";
                  testBackground.RunWorkerAsync();
               }
               else
               {
                  uiMessage.Failure("No test selected");
               }
            }
            else
            {
               testBackground.Folder = testFolder;
               testBackground.RunWorkerAsync();
            }
         }
         else if (_folder.Exception is (true, var exception))
         {
            uiMessage.Exception(exception);
         }
         else
         {
            uiMessage.Failure("Folder not selected");
         }
      };
      uiRun.ClickText = "Run tests";

      uiGenerate.Button("Generate Expected Texts");
      uiGenerate.KeyDownCaption = new KeyDownCapture.ControlKey("Overwrite Expected Texts");
      uiGenerate.Click += (_, _) =>
      {
         var _folder = getFolder();
         if (_folder is (true, var testFolder))
         {
            generateBackground.Folder = testFolder;
            generateBackground.Overwrite = uiGenerate.IsKeyDown;
         }
         else if (_folder.Exception is (true, var exception))
         {
            uiMessage.Exception(exception);
         }
         else
         {
            uiMessage.Failure("Folder not selected");
         }
      };

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 100f + 300 + 300;
      _ = builder.Row + 50f + 50f + 50;
      builder.SetUp();

      (builder + listViewTests).SpanCol(3).Row();
      (builder + textSource).SpanCol(3).Row();
      (builder + uiMessage).Next();
      (builder + uiRun).Next();
      (builder + uiGenerate).Row();
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

   protected void Tests_Load(object sender, EventArgs e)
   {
      Show();
      Application.DoEvents();

      if (getFolder() is (true, var testFolder))
      {
         var progress = new MessageEvent<string>();
         var finalized = new MessageEvent<FolderName>();
         TestBackground.LoadListView(testFolder, listViewTests, progress, finalized);
      }
   }

   protected void listViewTests_SelectedIndexChanged(object sender, EventArgs e)
   {
      try
      {
         if (getFolder() is (true, var testFolder) && listViewTests.SelectedItem() is (true, var item))
         {
            var resultFile = testFolder + $"{item.Text}.text";
            using var stream = resultFile.ReadingStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            textSource.Text = reader.ReadToEnd();
            /*var _text = resultFile.TryTo.GetText(Encoding.UTF8);
         if (_text is (true, var text))
         {
            textSource.Text = text;
         }
         else
         {
            textSource.Text = "";
            uiMessage.Exception(_text.Exception);
         }*/
         }
      }
      catch (Exception exception)
      {
         uiMessage.Exception(exception);
      }
   }
}