using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class YieldSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      var hasIteratorLabel = newLabel("has-iterator");
      var topLabel = newLabel("top");
      var bottomLabel = newLabel("bottom");
      var endLabel = newLabel("end");

      builder.HasIterator();
      builder.GoToIfTrue(hasIteratorLabel);
      builder.Yield();
      builder.GoTo(endLabel);

      builder.Label(hasIteratorLabel);
      builder.GetIterator(false);

      builder.Label(topLabel);
      builder.Dup();
      builder.SendMessage("next()", 0);
      builder.GoToIfNil(bottomLabel);
      builder.Yield();
      builder.GoTo(topLabel);

      builder.Label(bottomLabel);
      builder.Drop();

      builder.Label(endLabel);
      builder.NoOp();
   }

   public override Precedence Precedence => Precedence.PrefixOperator;

   public override Arity Arity => Arity.Prefix;

   public override string ToString() => "&";
}