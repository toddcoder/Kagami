using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class SameSymbol(bool not) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.SendMessage("string".get());
      builder.SendMessage("lower()");
      builder.Swap();
      builder.SendMessage("string".get());
      builder.SendMessage("lower()");
      builder.Swap();
      builder.Compare();
      builder.IsZero();
      if (not)
      {
         builder.PushBoolean(false);
         builder.Equal();
      }
   }

   public override Precedence Precedence => Precedence.Boolean;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => $"{(not ? "not " : "")}same";
}