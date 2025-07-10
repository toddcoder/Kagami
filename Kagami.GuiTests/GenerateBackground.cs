using Core.Computers;
using Core.Monads;
using Kagami.Library;

namespace Kagami.GuiTests;

public class GenerateBackground(Either<FolderName, FileName> source, ListView listView) : TestBackground(source, listView)
{
   public bool Overwrite { get; set; }

   public override void OnFile(FileName sourceFile)
   {
      var testFolder = sourceFile.Folder;
      var testName = sourceFile.Name;
      var expectedFile = testFolder + $"{testName}.expected.txt";

      if (expectedFile && !Overwrite)
      {
         return;
      }

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