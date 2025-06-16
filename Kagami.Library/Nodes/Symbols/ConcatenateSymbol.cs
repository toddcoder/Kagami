using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class ConcatenateSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.Concatenate();

   public override Precedence Precedence => Precedence.Concatenate;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => "~";
}