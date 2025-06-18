using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class CommaSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.NewSequence();

   public override Precedence Precedence => Precedence.Comma;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => ",";
}