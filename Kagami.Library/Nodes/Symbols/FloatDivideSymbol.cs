using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class FloatDivideSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.Divide();

   public override Precedence Precedence => Precedence.MultiplyDivide;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => "/";
}