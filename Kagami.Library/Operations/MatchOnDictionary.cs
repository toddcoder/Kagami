using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class MatchOnDictionary : TwoOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y)
      {
         if (y is Dictionary dictionary)
         {
            var hash = dictionary.InternalHash;
            var bindings = new Hash<string, IObject>();
            foreach (var (key, value) in hash)
               if (x.Match(key, bindings))
               {
                  Machine.Current.CurrentFrame.Fields.SetBindings(bindings, true, false);
                  return value.Matched();
               }

            return "No match".FailedMatch<IObject>();
         }
         else
            return failedMatch<IObject>(unableToConvert(y.Image, "Dictionary"));
      }

      public override string ToString() => "match.on.dictionary";
   }
}