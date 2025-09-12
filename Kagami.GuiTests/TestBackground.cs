using Core.Applications.Messaging;
using Core.Computers;
using Core.DataStructures;
using Core.Monads;
using Core.WinForms;
using Core.WinForms.Components;
using Kagami.Library;
using static Core.Monads.MonadFunctions;

namespace Kagami.GuiTests;

public class TestBackground(Either<FolderName, FileName> source, ListView listView) : Background
{
   protected Either<FolderName, FileName> source = source;
   protected Maybe<FolderName> _testFolder = nil;

   public readonly MessageEvent<string> Progress = new();
   public readonly MessageEvent FolderNotFound = new();
   public readonly MessageEvent<string[]> Results = new();

   public FolderName Folder
   {
      set => source = value;
   }

   public FileName File
   {
      set => source = value;
   }

   public override void DoWork()
   {
      switch (source.ToObject())
      {
         case FolderName folder:
            _testFolder = folder;
            OnFolder(folder);
            break;
         case FileName file:
            _testFolder = file.Folder;
            OnFile(file);
            break;
      }
   }

   public override void RunWorkerCompleted()
   {
      if (_testFolder is (true, var testFolder))
      {
      }
      else
      {
         FolderNotFound.Invoke();
         return;
      }

      LoadListView(testFolder, listView, Progress, Results);
   }

   public static void LoadListView(FolderName testFolder, ListView listView, MessageEvent<string> progress, MessageEvent<string[]> results)
   {
      var expected = 0;
      var noExpected = 0;
      var passed = 0;
      var failed = 0;
      var noResult = 0;

      try
      {
         listView.BeginUpdate();
         listView.Items.Clear();

         foreach (var file in testFolder.Files.Where(file => file.Extension == ".kagami").OrderBy(file => file.Name))
         {
            var item = listView.Items.Add(file.Name);
            item.UseItemStyleForSubItems = false;
            item.ForeColor = Color.White;
            item.BackColor = Color.Blue;

            var expectedFile = file.Folder + $"{file.Name}.expected.txt";
            if (expectedFile)
            {
               var expectedSubItem = item.SubItems.Add("Expected");
               expectedSubItem.ForeColor = Color.White;
               expectedSubItem.BackColor = Color.Green;
               expected++;
            }
            else
            {
               var expectedSubItem = item.SubItems.Add("Not Expected");
               expectedSubItem.ForeColor = Color.Black;
               expectedSubItem.BackColor = Color.Gold;
               noExpected++;
            }

            var resultFile = file.Folder + $"{file.Name}.txt";
            if (resultFile)
            {
               if (anyDifferentLines(resultFile, expectedFile))
               {
                  var resultSubItem = item.SubItems.Add("Failed");
                  resultSubItem.ForeColor = Color.Black;
                  resultSubItem.BackColor = Color.Gold;
                  failed++;
               }
               else
               {
                  var resultSubItem = item.SubItems.Add("Passed");
                  resultSubItem.ForeColor = Color.White;
                  resultSubItem.BackColor = Color.Green;
                  passed++;
               }
            }
            else
            {
               var resultSubItem = item.SubItems.Add("No Result");
               resultSubItem.ForeColor = Color.Black;
               resultSubItem.BackColor = Color.Gold;
               noResult++;
            }

            progress.Invoke($"{file.Name}...");
         }
      }
      finally
      {
         listView.AutoSizeColumns();
         listView.EndUpdate();
         results.Invoke([$"Expected {expected}", $"No expected {noExpected}", $"Passed {passed}", $"Failed {failed}", $"No result {noResult}"]);
      }
   }

   public virtual void OnFolder(FolderName sourceFolder)
   {
      var testFiles = sourceFolder.Files.Where(file => file.Extension == ".kagami");
      foreach (var file in testFiles)
      {
         OnFile(file);
      }
   }

   public virtual void OnFile(FileName sourceFile)
   {
      var testFolder = sourceFile.Folder;
      var testName = sourceFile.Name;
      var outputFile = testFolder + $"{testName}.txt";
      outputFile.TryTo.Delete();
      var expectedFile = testFolder + $"{testName}.expected.txt";

      using var context = new TestContext(outputFile);
      var compiler = new Compiler(sourceFile.Text, new CompilerConfiguration(), context);
      var _machine = compiler.Generate();
      if (_machine is (true, var machine))
      {
         var _result = machine.Execute();
         if (_result is (true, var result))
         {
            context.PrintLine($"{result.Image} | {result.ClassName}");
            compareFiles(outputFile, expectedFile);
         }
         else
         {
            context.PrintLine($"Execute error: {_result.Exception.Message}");
         }
      }
      else
      {
         context.PrintLine($"Generate error: {_machine.Exception.Message}");
      }

      Progress.Invoke(sourceFile.Name);
   }

   protected void compareFiles(FileName outputFile, FileName expectedFile)
   {
      try
      {
         Console.WriteLine(outputFile.Name);
         var outputLines = outputFile.Lines;
         var outputQueue = new MaybeQueue<string>(outputLines);

         var expectedLines = expectedFile.Lines;
         var expectedQueue = new MaybeQueue<string>(expectedLines);

         while (outputQueue.Dequeue() is (true, var outputLine) && expectedQueue.Dequeue() is (true, var expectedLine))
         {
            if (outputLine != expectedLine)
            {
               Console.WriteLine($"   O:{outputLine}");
               Console.WriteLine($"   E:{expectedLine}");
            }
         }
      }
      catch (Exception exception)
      {
         Console.WriteLine(exception.Message);
      }
   }

   protected static bool anyDifferentLines(FileName outputFile, FileName expectedFile)
   {
      try
      {
         var outputLines = outputFile.Lines;
         var expectedLines = expectedFile.Lines;
         if (outputLines.Length != expectedLines.Length)
         {
            return true;
         }

         for (var i = 0; i < outputLines.Length; i++)
         {
            if (outputLines[i] != expectedLines[i])
            {
               return true;
            }
         }

         return false;
      }
      catch
      {
         return true;
      }
   }
}