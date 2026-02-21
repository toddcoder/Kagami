using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class CountSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      var lambdaLabel = newLabel("lambda");
      var endLabel = newLabel("end");

      builder.IsClass("Lambda", false);
      builder.GoToIfTrue(lambdaLabel);
      builder.PushString("of");
      builder.Swap();
      builder.NewNameValue();
      builder.SendMessage("count(of:_)", 1);
      builder.GoTo(endLabel);

      builder.Label(lambdaLabel);
      builder.SendMessage("count(_<Lambda>)", 1);

      builder.Label(endLabel);
      builder.NoOp();
   }

   public override Precedence Precedence => Precedence.ChainedOperator;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => "#";
}