using System.Linq;
using Kagami.Library.Operations;
using Core.Enumerables;

namespace Kagami.Library.Nodes.Symbols
{
   public class IndexerSymbol : Symbol
   {
      Expression[] arguments;

      public IndexerSymbol(Expression[] arguments) => this.arguments = arguments;

      public override Precedence Precedence => Precedence.SendMessage;

      public override Arity Arity => Arity.Postfix;

      public override void Generate(OperationsBuilder builder)
      {
	      if (arguments.Length == 1)
		      arguments[0].Generate(builder);
	      else
	      {
		      arguments[0].Generate(builder);
		      arguments[1].Generate(builder);
				builder.NewInternalList();

				foreach (var argument in arguments.Skip(2))
				{
					argument.Generate(builder);
					builder.NewInternalList();
				}
	      }

	      builder.SendMessage("[](_)", 1);
      }

      public override string ToString() => $"[{arguments.Stringify()}]";
   }
}