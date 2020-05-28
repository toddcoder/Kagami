using System;
using Core.Assertions;
using Kagami.Library;
using Kagami.Library.Runtime;
using Core.Monads;
using static System.Console;
using static Core.Assertions.AssertionFunctions;

namespace Kagami
{
   public class Context : IContext
   {
      Putter putter;

      public Context() => putter = new Putter();

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

      public IResult<string> ReadLine()
      {
         var line = Console.ReadLine();
         return assert(() => line).Must().Not.BeNull().OrFailure("Input cancelled");
      }

      public bool Cancelled() => KeyAvailable && ReadKey().Key == ConsoleKey.Escape;

      public void Peek(string message, int index) { }
   }
}