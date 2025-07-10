using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class NotSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      var intLabel = newLabel("int");
      var endLabel = newLabel("end");

      builder.IsClass("Int", false);
      builder.GoToIfTrue(intLabel);

      builder.PushBoolean(false);
      builder.Equal();
      builder.GoTo(endLabel);

      builder.Label(intLabel);
      builder.Not();

      builder.Label(endLabel);
      builder.NoOp();
   }

   public override Precedence Precedence => Precedence.PrefixOperator;

   public override Arity Arity => Arity.Prefix;

   public override string ToString() => "not";
}