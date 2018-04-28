using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class LessThanSymbol : Symbol
   {
      public override void Generate(OperationsBuilder builder)
      {
         builder.Compare();
         builder.IsNegative();
      }

      public override Precedence Precedence => Precedence.Boolean;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => "<";
   }
}