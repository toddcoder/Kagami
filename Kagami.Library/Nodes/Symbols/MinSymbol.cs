using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class MinSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      var endLabel = newLabel("end");
      var negLabel = newLabel("neg");

      builder.Dup2();
      builder.Compare();
      builder.IsNegative();
      builder.GoToIfTrue(endLabel);

      builder.Label(negLabel);
      builder.Swap();

      builder.Label(endLabel);
      builder.Drop();
   }

   public override Precedence Precedence => Precedence.Boolean;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => "min";
}