using Core.Matching;
using Core.Monads;
using Core.Numbers;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public class FieldsIterator : Iterator
{
   protected string input;
   protected Core.Matching.Pattern pattern;
   protected bool matched;
   protected MatchResult matchResult = MatchResult.Empty;
   protected int matchIndex;

   public FieldsIterator(KString kString) : base(kString)
   {
      input = kString.Value;
      pattern = "-/s+";
   }

   public FieldsIterator(KString kString, Regex regex) : base(kString)
   {
      input = kString.Value;
      pattern = regex.Pattern;
   }

   public override Maybe<IObject> Next()
   {
      if (!matched)
      {
         matched = true;
         var _result = input.Matches(pattern);
         if (_result is (true, var result))
         {
            matchResult = result;
         }
         else
         {
            return nil;
         }
      }

      return matchIndex < matchResult.MatchCount ? KString.StringObject(matchResult[matchIndex++]).Some() : nil;
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