using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class FormatSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      var imageLabel = newLabel("image");
      var endLabel = newLabel("end");

      builder.Dup();
      builder.PushString("i");
      builder.Equal();
      builder.GoToIfTrue(imageLabel);
      builder.SendMessage("format(_)", 1);
      builder.GoTo(endLabel);
      builder.Label(imageLabel);
      builder.Drop();
      builder.SendMessage("image".get(), 0);
      builder.Label(endLabel);
      builder.NoOp();
   }

   public override Precedence Precedence => Precedence.Format;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => @"\\";
}