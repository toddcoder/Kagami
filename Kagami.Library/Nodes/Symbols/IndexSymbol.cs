using Kagami.Library.Operations;
using Standard.Types.Enumerables;

namespace Kagami.Library.Nodes.Symbols
{
   public class IndexSymbol : Symbol
   {
      Expression[] arguments;

      public IndexSymbol(Expression[] arguments) => this.arguments = arguments;

      public override Precedence Precedence => Precedence.SendMessage;

      public override Arity Arity => Arity.Postfix;

      public override void Generate(OperationsBuilder builder) => builder.SendMessage("[]", arguments);

      public override string ToString() => $"[{arguments.Listify()}]";
   }
}