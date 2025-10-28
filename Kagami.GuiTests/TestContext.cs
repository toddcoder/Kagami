using Core.Computers;
using Core.Monads;
using Kagami.Library;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.GuiTests;

public class TestContext : IContext, IDisposable, IAsyncDisposable
{
   protected FileStream stream;
   protected TextWriter writer;
   protected Putter putter = new();

   public TestContext(FileName resultFile)
   {
      stream = resultFile.WritingStream();
      writer = new StreamWriter(stream) { AutoFlush = true };
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

   public Result<string> ReadLine() => nil;

   public bool Cancelled() => false;

   public int WriteCount { get; set; }

   public void Dispose()
   {
      stream.Dispose();
   }

   public async ValueTask DisposeAsync()
   {
      await stream.DisposeAsync();
   }
}