using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class IsSymbol : Symbol
   {
      Expression comparisand;
      Expression expression;

      public IsSymbol(Expression comparisand, Expression expression)
      {
	      this.comparisand = comparisand;
	      this.expression = expression;
      }

      public override void Generate(OperationsBuilder builder)
      {
			builder.PushFrameWithValue();

         comparisand.Generate(builder);
         builder.Match();

			expression.Generate(builder);

			builder.PopFrameWithValue();
      }

      public override Precedence Precedence => Precedence.Boolean;


      public override Arity Arity => Arity.Binary;

      public override string ToString() => $"?? {comparisand} : {expression}";
   }
}