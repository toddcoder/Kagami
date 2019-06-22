using Core.Monads;
using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols
{
   public class IsSymbol : Symbol
   {
      Expression comparisand;
      Expression expression;
      IMaybe<Expression> elseExpression;

      public IsSymbol(Expression comparisand, Expression expression, IMaybe<Expression> elseExpression)
      {
	      this.comparisand = comparisand;
	      this.expression = expression;
	      this.elseExpression = elseExpression;
      }

      public override void Generate(OperationsBuilder builder)
      {
	      var elseLabel = newLabel("else");
	      var endLabel = newLabel("end");

			builder.PushFrameWithValue();

         comparisand.Generate(builder);
         builder.Match();

         builder.GoToIfFalse(elseLabel);

			expression.Generate(builder);
			builder.GoTo(endLabel);

			builder.Label(elseLabel);

			if (elseExpression.If(out var elseExpressionValue))
			{
				elseExpressionValue.Generate(builder);
			}

			builder.Label(endLabel);

			builder.PopFrameWithValue();
      }

      public override Precedence Precedence => Precedence.Boolean;


      public override Arity Arity => Arity.Binary;

      public override string ToString() => $"?? {comparisand} : {expression}";
   }
}