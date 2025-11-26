using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class MaybeSymbol2(Expression expression) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      var noneLabel = newLabel("none");
      var endLabel = newLabel("end");
      builder.GoToIfFalse(noneLabel);

      expression.Generate(builder);
      builder.Some();
      builder.GoTo(endLabel);

      builder.Label(noneLabel);
      builder.PushNil();

      builder.Label(endLabel);
      builder.NoOp();
   }

   public override Precedence Precedence => Precedence.ChainedOperator;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => "??";
}