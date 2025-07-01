using Core.Computers;
using Core.DataStructures;
using Core.Strings;
using Kagami.Library;

namespace Kagami.Tests;

public class Tests(FolderName testFolder)
{
   public void RunAllTests()
   {
      var testNames = testFolder.Files
         .Where(file => file.Extension == ".kagami")
         .Select(file => file.Name);
      foreach (var testName in testNames)
      {
         runTest(testName);
      }
   }

   protected void runTest(string testName)
   {
      var sourceFile = testFolder + $"{testName}.kagami";
      var outputFile = testFolder + $"{testName}.txt";
      var expectedFile = testFolder + $"{testName}.expected.txt";

      using var context = new TestContext(outputFile);
      var compiler = new Compiler(sourceFile.Text, new CompilerConfiguration(), context);
      var _machine = compiler.Generate();
      if (_machine is (true, var machine))
      {
         var _result = machine.Execute();
         if (_result is (true, var result))
         {
            context.PrintLine($"{result} | {result.ClassName}");
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
         var outputLines = outputFile.Lines;
         var outputQueue = new MaybeQueue<string>(outputLines);

         var expectedLines = expectedFile.Lines;
         var expectedQueue = new MaybeQueue<string>(expectedLines);

         while (outputQueue.Dequeue() is (true, var outputLine) && expectedQueue.Dequeue() is (true, var expectedLine))
         {
            if (outputLine == expectedLine)
            {
               Console.WriteLine($"E: {outputLine.Truncate(30)}=={expectedLine.Truncate(30)}");
            }
            else
            {
               Console.WriteLine($"N: {outputLine.Truncate(30)}!={expectedLine.Truncate(30)}");
            }
         }
      }
      catch (Exception exception)
      {
         Console.WriteLine(exception.Message);
      }
   }

   protected void generateExpectedText(string testName)
   {
      var sourceFile = testFolder + $"{testName}.kagami";
      var expectedFile = testFolder + $"{testName}.expected.txt";

      using var context = new TestContext(expectedFile);
      var compiler = new Compiler(sourceFile.Text, new CompilerConfiguration(), context);
      var _machine = compiler.Generate();
      if (_machine is (true, var machine))
      {
         var _result = machine.Execute();
         if (_result is (true, var result))
         {
            context.PrintLine($"{result} | {result.ClassName}");
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