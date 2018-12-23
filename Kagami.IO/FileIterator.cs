using System.IO;
using Kagami.Library.Objects;
using Standard.Types.Monads;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.IO
{
   public class FileIterator : Iterator
   {
      TextReader reader;

      public FileIterator(File file) : base(file) => reader = file.Reader();

      public override IMaybe<IObject> Next()
      {
         var line = reader.ReadLine();
         if (line == null)
         {
            reader?.Dispose();
            return none<IObject>();
         }
         else
            return String.StringObject(line).Some();
      }

      public override IMaybe<IObject> Peek() => Next();
   }
}