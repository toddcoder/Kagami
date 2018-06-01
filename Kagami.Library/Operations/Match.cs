using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Booleans;
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
         return machine.GlobalFrame.Sys.Match(mutable, strict, x, y, assign).Match();
      }

      public override string ToString()
      {
         if (assign)
            return "match";
         else
            return $"match({mutable.Extend("mutable", "immutable")}, {strict.Extend("strict", "loose")})";
      }
   }
}