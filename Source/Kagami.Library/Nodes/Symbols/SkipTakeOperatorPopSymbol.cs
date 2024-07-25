using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class SkipTakeOperatorPopSymbol : Symbol
   {
      public override void Generate(OperationsBuilder builder) => builder.SkipTake();

      public override Precedence Precedence => Precedence.SendMessage;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => ";";
   }
}