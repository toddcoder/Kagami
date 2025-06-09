using Core.Applications.CommandProcessing;
using Core.Collections;
using Core.Computers;
using Core.Enumerables;
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
      if (File is (true, var file))
      {
         var context = new ConsoleContext();
         var compiler = new Compiler(file.Text, compilerConfiguration, context);
         _machine = compiler.Generate();
         if (_machine is (true, var machine))
         {
            var _result = machine.Execute();
            if (_result is (true, var result) && !context.AnythingPrinted)
            {
               System.Console.WriteLine($"{result.Image} | {result.ClassName}");
            }
            else
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

   [Command("show")]
   public void Show()
   {
      if (File is (true, var file))
      {
         var context = new ConsoleContext();
         var compiler = new Compiler(file.Text, compilerConfiguration, context);
         _machine = compiler.Generate();
         if (_machine && compiler.Operations is (true, var operations))
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

   [Command("repl")]
   public void Repl()
   {
      List<string> source = [];
      var context = new ConsoleContext();
      while (true)
      {
         System.Console.Write("kagami> ");
         var line = System.Console.ReadLine();
         if (line is null or "quit")
         {
            break;
         }
         else
         {
            switch (line)
            {
               case "clear":
                  source.Clear();
                  System.Console.Clear();
                  continue;
               case "list":
                  System.Console.WriteLine(source.ToString("\n"));
                  continue;
               case "reset":
                  source.Clear();
                  _machine = nil;
                  System.Console.WriteLine("Reset complete.");
                  continue;
            }
         }

         source.Add(line);
         var compiler = new Compiler(source.ToString("\n"), compilerConfiguration, context);
         _machine = compiler.Generate();
         if (_machine is (true, var machine))
         {
            var _result = machine.Execute();
            if (_result is (true, var result) && !context.AnythingPrinted)
            {
               System.Console.WriteLine($"{result.Image} | {result.ClassName}");
            }
            else
            {
               source.RemoveAt(source.Count - 1);
               System.Console.WriteLine(_result.Exception.Message);
            }
         }
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