using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class ForcedFloatDivide : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.PushFloat(1.0);
      builder.Multiply();
      builder.FloatDivide();
   }

   public override Precedence Precedence => Precedence.MultiplyDivide;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => "* 1.0 /";
}