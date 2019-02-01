using Kagami.Library.Operations;
using Standard.Types.Booleans;

namespace Kagami.Library.Nodes.Symbols
{
   public class IsSymbol : Symbol
   {
      Expression comparisand;
      bool not;

      public IsSymbol(Expression comparisand, bool not)
      {
	      this.comparisand = comparisand;
	      this.not = not;
      }

      public override void Generate(OperationsBuilder builder)
      {
         comparisand.Generate(builder);
         builder.Match();
			if (not)
				builder.Not();
      }

      public override Precedence Precedence => Precedence.Boolean;


      public override Arity Arity => Arity.Binary;

      public override string ToString() => $"is {not.Extend("not ")}{comparisand}";
   }
}