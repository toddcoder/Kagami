using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class ConsSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.ToArguments(2);
      builder.NewValue("List", args => List.Cons(args[0], (List)args[1]));
   }

   public override Precedence Precedence => Precedence.Concatenate;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => "::";

   public override bool LeftToRight => false;
}