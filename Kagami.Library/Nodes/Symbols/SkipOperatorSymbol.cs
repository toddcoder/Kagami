using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class SkipOperatorSymbol : Symbol
   {
      public override void Generate(OperationsBuilder builder)
      {
         builder.PushNone();
         builder.SkipTake();
      }

      public override Precedence Precedence => Precedence.PostfixOperator;

      public override Arity Arity => Arity.Postfix;

      public override string ToString() => ":*";
   }
}