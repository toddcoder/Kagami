using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class ForwardReductionSymbol(string operation, Expression expression) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      expression.Generate(builder);
      builder.ForwardReduction(operation);
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;
}