using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class LessThanEqualSymbol : Symbol
   {
      public override void Generate(OperationsBuilder builder)
      {
         builder.Dup2();
         builder.Compare();
         builder.IsNegative();
         builder.Rotate(3);
         builder.Equal();
         builder.Or();
      }

      public override Precedence Precedence => Precedence.Boolean;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => "<=";
   }
}