using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class MatchSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.Match();

   public override Precedence Precedence => Precedence.Boolean;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => "|=";
}