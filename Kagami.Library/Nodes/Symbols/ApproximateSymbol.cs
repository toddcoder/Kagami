using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class ApproximateSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.Abs();
      builder.Swap();
      builder.Abs();
      builder.Subtract();
      builder.Abs();
      builder.PushFloat(1.0e-15);
      builder.Compare();
      builder.IsNegative();
   }

   public override Precedence Precedence => Precedence.Boolean;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => "=~";
}