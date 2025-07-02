using Core.Monads;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Packages;

public class FileIterator : Iterator
{
   protected TextReader reader;

   public FileIterator(File file) : base(file)
   {
      reader = file.Reader();
   }

   public override Maybe<IObject> Next()
   {
      var line = reader.ReadLine();
      if (line == null)
      {
         reader.Dispose();
         return nil;
      }
      else
      {
         return KString.StringObject(line).Some();
      }
   }

   public override Maybe<IObject> Peek() => Next();
}