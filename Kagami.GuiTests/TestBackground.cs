using Core.Computers;
using Core.DataStructures;
using Core.Monads;
using Core.WinForms.Components;
using Kagami.Library;

namespace Kagami.GuiTests;

public class TestBackground(Either<FolderName, FileName> source) : Background
{
   protected void testFolder(FolderName sourceFolder)
   {
      var testFiles = sourceFolder.Files.Where(file => file.Extension == ".kagami");
      foreach (var file in testFiles)
      {
         testFile(file);
      }
   }

   protected void testFile(FileName sourceFile)
   {
      var testFolder = sourceFile.Folder;
      var testName = sourceFile.Name;
      var outputFile = testFolder + $"{testName}.txt";
      outputFile.TryTo.Delete();
      var expectedFile = testFolder + $"{testName}.expected.txt";

      var context = new TestContext(outputFile);
      var compiler = new Compiler(sourceFile.Text, new CompilerConfiguration(), context);
      var _machine = compiler.Generate();
      if (_machine is (true, var machine))
      {
         var _result = machine.Execute();
         if (_result is (true, var result))
         {
            context.PrintLine($"{result.Image} | {result.ClassName}");
            context.Dispose();
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
}