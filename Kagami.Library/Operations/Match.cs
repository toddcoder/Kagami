using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Booleans;
using Standard.Types.Collections;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
   public class Match : TwoOperandOperation
   {
      bool mutable;
      bool strict;
      bool assign;

      public Match(bool mutable, bool strict)
      {
         this.mutable = mutable;
         this.strict = strict;
         assign = false;
      }

      public Match() => assign = true;

      public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y)
      {
         var bindings = new Hash<string, IObject>();
         if (x.Match(y, bindings))
         {
            if (assign)
               machine.CurrentFrame.Fields.AssignBindings(bindings);
            else
               machine.CurrentFrame.Fields.SetBindings(bindings, mutable, strict);

            return Boolean.True.Matched();
         }
         else
            return Boolean.False.Matched();
      }

      public override string ToString()
      {
         if (assign)
            return $"match";
         else
            return $"match({mutable.Extend("mutable", "immutable")}, {strict.Extend("strict", "loose")})";
      }
   }
}