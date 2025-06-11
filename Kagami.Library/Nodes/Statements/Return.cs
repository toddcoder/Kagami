using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Core.Monads;

namespace Kagami.Library.Nodes.Statements;

public class Return : Statement
{
   protected Expression expression;
   protected Maybe<TypeConstraint> _typeConstraint;

   public Return(Expression expression, Maybe<TypeConstraint> _typeConstraint)
   {
      this.expression = expression;
      this._typeConstraint = _typeConstraint;
   }

   public override void Generate(OperationsBuilder builder)
   {
      builder.Return(expression, this, _typeConstraint);
   }

   public override string ToString() => $"return {expression}";
}