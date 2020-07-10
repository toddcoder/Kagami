using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class TakeOperatorSymbol :Symbol
   {
      public override void Generate(OperationsBuilder builder)
      {
         builder.PushInt(0);
         builder.Swap();
         builder.SkipTake();
      }

      public override Precedence Precedence => Precedence.PrefixOperator;

      public override Arity Arity => Arity.Prefix;

      public override string ToString() => "*;";
   }
}