using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class ConcatenationSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      var stringLabel = newLabel("string");
      var endLabel = newLabel("end");

      builder.Swap();
      builder.RespondsTo("~(_)");
      builder.GoToIfFalse(stringLabel);
      builder.Swap();
      builder.SendMessage("~(_)", 1);
      builder.GoTo(endLabel);

      builder.Label(stringLabel);
      builder.Swap();
      builder.String();
      builder.Swap();
      builder.String();
      builder.Swap();
      builder.SendMessage("~(_)", 1);

      builder.Label(endLabel);
      builder.NoOp();

   }

   public override Precedence Precedence => Precedence.Concatenate;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => "~";
}