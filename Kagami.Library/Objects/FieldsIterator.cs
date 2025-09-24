using Core.Matching;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public class FieldsIterator : Iterator
{
   protected string input;
   protected Core.Matching.Pattern pattern;
   protected string[] parts;

   public FieldsIterator(KString kString) : base(kString)
   {
      input = kString.Value;
      pattern = "/s+";
      parts = input.Unjoin(pattern);
   }

   public FieldsIterator(KString kString, Regex regex) : base(kString)
   {
      input = kString.Value;
      pattern = regex.CorePattern;
      parts = input.Unjoin(pattern);
   }

   public override Maybe<IObject> Next() => index < parts.Length ? KString.StringObject(parts[index++]).Some() : nil;

   public override IObject this[int index]
   {
      get
      {
         index = wrapIndex(index, parts.Length);
         return index < parts.Length ? KString.StringObject(parts[index]) : KString.Empty;
      }
   }
}