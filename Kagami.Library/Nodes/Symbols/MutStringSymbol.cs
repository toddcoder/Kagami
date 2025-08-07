using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class MutStringSymbol : Symbol
{
   protected string mutString;

   public MutStringSymbol(string mutString)
   {
      this.mutString = mutString;
   }

   public override void Generate(OperationsBuilder builder) => builder.NewMutString(mutString);

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => new MutString(mutString).Image;
}