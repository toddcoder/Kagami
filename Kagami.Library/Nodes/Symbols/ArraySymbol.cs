using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class ArraySymbol : Symbol, IHasExpression
{
   protected Expression expression;
   protected Maybe<TypeConstraint> _typeConstraint;

   public ArraySymbol(Expression expression, Maybe<TypeConstraint> _typeConstraint)
   {
      this.expression = expression;
      this._typeConstraint = _typeConstraint;
   }

   public override void Generate(OperationsBuilder builder)
   {
      expression.Generate(builder);
      builder.NewArray(_typeConstraint);
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"[{expression}]";

   public Expression Expression => expression;
}