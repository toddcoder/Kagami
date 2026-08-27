using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using Kagami.Library.Parsers.Statements;

namespace Kagami.Library.Nodes.Statements;

public class BuilderYield(BuilderState builderState, Expression expression) : BuilderStatement(builderState), IHasExpression
{
   public override void Generate(OperationsBuilder builder)
   {
      Prefix(builder);
   }

   public Expression Expression => expression;
}