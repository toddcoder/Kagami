using System;
using Core.Applications;
using Kagami.Library;
using Kagami.Library.Runtime;
using Core.Monads;
using static System.Console;

namespace Kagami
{
   internal class Program : CommandLine, IContext
   {
      Putter putter;

      public Program() => putter = new Putter();

      static void Main(string[] args)
      {
         var program = new Program();
         program.Run(args);
      }

      public override void Execute(Arguments arguments)
      {
         if (arguments[0].FileName.If(out var sourceFile))
            if (sourceFile.TryTo.Text.If(out var source, out var exception))
            {
               var configuration = new CompilerConfiguration { ShowOperations = false, Tracing = false};
               var compiler = new Compiler(source, configuration, this);
               var result =
                  from machine in compiler.Generate().OnSuccess(m =>
                  {
                     if (configuration.ShowOperations)
                        WriteLine(m.Operations);
                  })
                  from executed in machine.Execute()
                  select executed;
               if (result.IsFailed)
                  WriteLine($"Exception: {result.Exception}");
            }
            else
               WriteLine($"Exception: {exception}");
      }

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

      public IMaybe<string> ReadLine() => Console.ReadLine().SomeIfNotNull();

      public bool Cancelled() => KeyAvailable && ReadKey().Key == ConsoleKey.Escape;

      public void Peek(string message, int index) { }
   }
}