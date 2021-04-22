using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Core.Booleans;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Nodes.Statements
{
   public class ExpressionStatement : Statement
   {
      protected Expression expression;
      protected bool returnExpression;
      protected IMaybe<TypeConstraint> _typeConstraint;

      public ExpressionStatement(Expression expression, bool returnExpression, IMaybe<TypeConstraint> typeConstraint)
      {
         this.expression = expression;
         this.returnExpression = returnExpression;
         _typeConstraint = typeConstraint;
      }

      public ExpressionStatement(Expression expression, bool returnExpression) : this(expression, returnExpression, none<TypeConstraint>())
      {
      }

      public ExpressionStatement(Symbol symbol, bool returnExpression, IMaybe<TypeConstraint> typeConstraint)
      {
         expression = new Expression(symbol);
         this.returnExpression = returnExpression;
         _typeConstraint = typeConstraint;
      }

      public ExpressionStatement(Symbol symbol, bool returnExpression) : this(symbol, returnExpression, none<TypeConstraint>())
      {
      }

      public override void Generate(OperationsBuilder builder)
      {
         expression.Generate(builder);
         builder.Peek(Index);
         if (returnExpression)
         {
            if (_typeConstraint.If(out var typeConstraint))
            {
               builder.ReturnType(true, typeConstraint);
            }
            else
            {
               builder.Return(true);
            }
         }
      }

      public Expression Expression => expression;

      public bool ReturnExpression => returnExpression;

      public override string ToString() => $"{returnExpression.Extend("return ")}{expression}";
   }
}