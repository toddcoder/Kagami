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
      protected Maybe<TypeConstraint> _typeConstraint;

      public ExpressionStatement(Expression expression, bool returnExpression, Maybe<TypeConstraint> _typeConstraint)
      {
         this.expression = expression;
         this.returnExpression = returnExpression;
         this._typeConstraint = _typeConstraint;
      }

      public ExpressionStatement(Expression expression, bool returnExpression) : this(expression, returnExpression, nil)
      {
      }

      public ExpressionStatement(Symbol symbol, bool returnExpression, Maybe<TypeConstraint> _typeConstraint)
      {
         expression = new Expression(symbol);
         this.returnExpression = returnExpression;
         this._typeConstraint = _typeConstraint;
      }

      public ExpressionStatement(Symbol symbol, bool returnExpression) : this(symbol, returnExpression, nil)
      {
      }

      public override void Generate(OperationsBuilder builder)
      {
         expression.Generate(builder);
         builder.Peek(Index);
         if (returnExpression)
         {
            if (_typeConstraint)
            {
               builder.ReturnType(true, _typeConstraint);
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