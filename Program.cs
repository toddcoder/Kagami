using System;
using System.Diagnostics;
using Core.Applications;
using Core.Assertions;
using Core.Computers;
using Core.Dates;
using Kagami.Library;
using Kagami.Library.Runtime;
using Core.Monads;
using static System.Console;
using static Core.Monads.MonadFunctions;

namespace Kagami;

internal class Program : CommandLineInterface, IContext
{
   protected Putter putter;

   public Program() => putter = new Putter();

   public static void Main()
   {
      using var program = new Program();
      program.Run();
   }

   [EntryPoint(EntryPointType.This)]
   public void EntryPoint()
   {
      if (Exec)
      {
         exec();
      }
   }

   protected void exec()
   {
      if (File is (true, var sourceFile))
      {
         var _source = sourceFile.TryTo.Text;
         if (_source is (true, var source))
         {
            var stopwatch = new Stopwatch();
            if (Stopwatch)
            {
               stopwatch.Start();
            }

            var configuration = new CompilerConfiguration { ShowOperations = ShowOps, Tracing = Trace };
            var compiler = new Compiler(source, configuration, this);
            var _result =
               from machine in compiler.Generate().OnSuccess(m =>
               {
                  if (configuration.ShowOperations)
                  {
                     WriteLine(m.Operations);
                  }
               })
               from executed in machine.Execute()
               select executed;
            if (!_result)
            {
               WriteLine($"Exception: {_result.Exception}");
            }

            if (Stopwatch)
            {
               stopwatch.Stop();
               WriteLine(stopwatch.Elapsed.ToLongString(true));
            }
         }
         else
         {
            WriteLine($"Exception: {_source.Exception}");
         }
      }
   }

   public bool Exec { get; set; }

   public Maybe<FileName> File { get; set; } = nil;

   public bool Stopwatch { get; set; }

   public bool ShowOps { get; set; }

   public bool Trace { get; set; }

   public void Print(string value)
   {
      putter.Reset();
      Write(value);
   }

   public void PrintLine(string value)
   {
      putter.Reset();
      WriteLine(value);
   }

   public void Put(string value) => Write(putter.Put(value));

   public Result<string> ReadLine()
   {
      var line = Console.ReadLine();
      return line.Must().Not.BeNull().OrFailure("Input cancelled");
   }

   public bool Cancelled() => KeyAvailable && ReadKey().Key == ConsoleKey.Escape;

   public void Peek(string message, int index)
   {
   }
}