using System;
using Kagami.Library;
using Kagami.Library.Runtime;
using Core.Monads;
using static System.Console;

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

      public IMaybe<string> ReadLine() => Console.ReadLine().SomeIfNotNull();

      public bool Cancelled() => KeyAvailable && ReadKey().Key == ConsoleKey.Escape;

      public void Peek(string message, int index) { }
   }
}