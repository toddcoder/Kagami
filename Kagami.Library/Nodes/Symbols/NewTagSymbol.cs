using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class NewTagSymbol(string tag) : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.NewTag(tag);

   public override Precedence Precedence => Precedence.PostfixOperator;

   public override Arity Arity => Arity.Postfix;
}