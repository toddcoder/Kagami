using Core.Assertions;
using Kagami.Library;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Monads;

namespace Kagami.Playground;

public class PlaygroundContext : IContext
{
   protected TextWriter writer;
   protected TextReader reader;
   protected Memo<int, string> peeks = new Memo<int, string>.Function(_ => "");
   protected bool cancelled;
   protected Putter putter = new();

   public PlaygroundContext(TextWriter writer, TextReader reader)
   {
      this.writer = writer;
      this.reader = reader;
   }

   public Machine Machine { get; set; }

   public Hash<int, string> Peeks => peeks;

   public void Print(string value)
   {
      putter.Reset();
      writer.Write(value);
   }

   public void PrintLine(string value)
   {
      putter.Reset();
      writer.WriteLine(value);
   }

   public void Put(string value) => writer.Write(putter.Put(value));

   public Result<string> ReadLine()
   {
      putter.Reset();
      var line = reader.ReadLine();

      return line.Must().Not.BeNull().OrFailure("Input cancelled");
   }

   public bool Cancelled()
   {
      Application.DoEvents();
      return cancelled;
   }

   public void Peek(string message, int index) => peeks[index] = message;

   public void ClearPeeks() => peeks.Clear();

   public void Cancel() => cancelled = true;

   public void Reset()
   {
      cancelled = false;
      putter.Reset();
   }
}