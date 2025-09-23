using Core.Monads;
using Core.Numbers;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public class FieldsIterator : Iterator
{
   protected string input;
   protected Core.Matching.Pattern pattern;
   protected IEnumerator<string> enumerator;

   public FieldsIterator(KString kString) : base(kString)
   {
      input = kString.Value;
      pattern = "-/s+";
      enumerator = splitOn(pattern, input).GetEnumerator();
   }

   public FieldsIterator(KString kString, Regex regex) : base(kString)
   {
      input = kString.Value;
      pattern = regex.CorePattern;
      enumerator = splitOn(pattern, input).GetEnumerator();
   }

   public override Maybe<IObject> Next()
   {
      if (enumerator.MoveNext())
      {
         return KString.StringObject(enumerator.Current).Some();
      }
      else
      {
         return nil;
      }
   }

   public override IObject this[int index]
   {
      get
      {
         if (index > -1)
         {
            Maybe<IObject> _next = nil;
            for (var i = 0; i < index; i++)
            {
               _next = Next();
            }

            return _next | (() => KString.Empty);
         }
         else
         {
            var list = List().ToList();
            index = wrapIndex(index, list.Count);

            return index.Between(0).Until(list.Count) ? list[index] : KString.Empty;
         }
      }
   }
}