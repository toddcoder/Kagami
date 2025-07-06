using Core.Computers;
using Core.DataStructures;
using Core.Strings;
using Kagami.Library;

namespace Kagami.Tests;

public class Tests
{
   public void RunAllTests(FolderName testFolder)
   {
      var testNames = testFolder.Files
         .Where(file => file.Extension == ".kagami");
      foreach (var testName in testNames)
      {
         RunTest(testName);
      }
   }

   public void GenerateExpectedTexts(FolderName testFolder)
   {
      var testNames = testFolder.Files
         .Where(file => file.Extension == ".kagami");
      foreach (var testName in testNames)
      {
         GenerateExpectedText(testName);
      }
   }

   public void RunTest(FileName sourceFile)
   {
      var testFolder = sourceFile.Folder;
      var testName = sourceFile.Name;
      var outputFile = testFolder + $"{testName}.txt";
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
               Console.WriteLine($"   N: {outputLine.Truncate(30)}!={expectedLine.Truncate(30)}");
            }
         }
      }
      catch (Exception exception)
      {
         Console.WriteLine(exception.Message);
      }
   }

   public void GenerateExpectedText(FileName sourceFile)
   {
      var testFolder = sourceFile.Folder;
      var testName = sourceFile.Name;
      var expectedFile = testFolder + $"{testName}.expected.txt";

      expectedFile.TryTo.Delete();
      using var context = new TestContext(expectedFile);
      var compiler = new Compiler(sourceFile.Text, new CompilerConfiguration(), context);
      var _machine = compiler.Generate();
      if (_machine is (true, var machine))
      {
         var _result = machine.Execute();
         if (_result is (true, var result))
         {
            context.PrintLine($"{result.Image} | {result.ClassName}");
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
}