using Core.Monads;
using Kagami.Library;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Console;

public class ConsoleContext : IContext
{
   protected Putter putter = new();
   protected bool cancelled;
   protected bool anythingPrinted;

   public ConsoleContext()
   {
      System.Console.CancelKeyPress += (_, _) => cancelled = true;
   }

   public bool AnythingPrinted => anythingPrinted;

   public void Print(string value)
   {
      putter.Reset();
      WriteCount = 0;
      System.Console.Write(value);
      anythingPrinted = true;
   }

   public void PrintLine(string value)
   {
      putter.Reset();
      WriteCount = 0;
      System.Console.WriteLine(value);
      anythingPrinted = true;
   }

   public void Put(string value) => System.Console.Write(putter.Put(value));

   public void Put(string value, string separator) => System.Console.Write(putter.Put(value, separator));

   public Result<string> ReadLine()
   {
      putter.Reset();
      WriteCount = 0;
      var line = System.Console.ReadLine();

      return line is not null ? line : fail("Null reference");
   }

   public bool Cancelled() => cancelled;

   public int WriteCount { get; set; }

   public void Reset()
   {
   }
}