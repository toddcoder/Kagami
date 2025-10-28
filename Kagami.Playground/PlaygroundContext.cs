using Kagami.Library;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Playground;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public class PlaygroundContext : IContext
{
   protected TextWriter writer;
   protected TextReader reader;
   protected bool cancelled;
   protected Putter putter = new();

   public PlaygroundContext(TextWriter writer, TextReader reader)
   {
      this.writer = writer;
      this.reader = reader;
   }

   public void Print(string value)
   {
      putter.Reset();
      WriteCount = 0;
      writer.Write(value);
   }

   public void PrintLine(string value)
   {
      putter.Reset();
      WriteCount = 0;
      writer.WriteLine(value);
   }

   public void Put(string value) => writer.Write(putter.Put(value));

   public void Put(string value, string separator) => writer.Write(putter.Put(value, separator));

   public Result<string> ReadLine()
   {
      putter.Reset();
      WriteCount = 0;
      var line = reader.ReadLine();

      return line ?? throw new NullReferenceException();
   }

   public bool Cancelled()
   {
      Application.DoEvents();
      return cancelled;
   }

   public int WriteCount { get; set; }

   public void Cancel() => cancelled = true;

   public void Reset()
   {
      cancelled = false;
      putter.Reset();
      WriteCount = 0;
   }
}