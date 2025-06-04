using Core.Applications.CommandProcessing;
using Core.Collections;
using Core.Computers;
using Core.Monads;
using Kagami.Library;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Console;

public class Processor : CommandProcessor
{
   protected Result<Machine> _machine = nil;
   protected CompilerConfiguration compilerConfiguration = new();

   public Processor() : base("Kagami.Console")
   {
   }

   public override void Initialize()
   {
      base.Initialize();

      compilerConfiguration = new CompilerConfiguration
      {
         ShowOperations = configuration.Maybe.Boolean("showOperations") | false,
         Tracing = configuration.Maybe.Boolean("tracing") | false
      };
   }

   [Switch("file", "File", "$file")]
   public Maybe<FileName> File { get; set; } = nil;

   [Command("run")]
   public void Compile()
   {
      if (File is (true, var fileName))
      {
         var context = new ConsoleContext();
         var compiler = new Compiler(fileName.Text, compilerConfiguration, context);
         _machine = compiler.Generate();
         if (_machine is (true, var machine))
         {
            var _result = machine.Execute();
            if (!_result)
            {
               System.Console.WriteLine(_result.Exception.Message);
            }
         }

         if (compilerConfiguration.ShowOperations && compiler.Operations is (true, var operations))
         {
            System.Console.WriteLine();
            System.Console.WriteLine("Operations:");
            System.Console.WriteLine(operations);
         }
      }
      else
      {
         System.Console.WriteLine("No file specified for compilation.");
      }
   }

   public override StringHash GetConfigurationDefaults()
   {
      StringHash hash = [];
      hash["showOperations"] = "false";
      hash["tracing"] = "false";

      return hash;
   }

   public override StringHash GetConfigurationHelp() => [];
}