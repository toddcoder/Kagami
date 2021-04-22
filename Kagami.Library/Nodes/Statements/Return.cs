using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Core.Monads;

namespace Kagami.Library.Nodes.Statements
{
   public class Return : Statement
   {
      protected Expression expression;
      protected IMaybe<TypeConstraint> typeConstraint;

      public Return(Expression expression, IMaybe<TypeConstraint> typeConstraint)
      {
         this.expression = expression;
         this.typeConstraint = typeConstraint;
      }

      public override void Generate(OperationsBuilder builder)
      {
         builder.Return(expression, this, typeConstraint);
      }

      public override string ToString() => $"return {expression}";
   }
}