using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Runtime;

namespace Kagami.Library.Nodes.Statements;

public class AssignToNewGuardedField : AssignToNewField
{
   protected LambdaSymbol predicate;
   protected Maybe<Expression> _failure;

   public AssignToNewGuardedField(bool mutable, string fieldName, Expression expression, Maybe<TypeConstraint> _typeConstraint, bool isHidden,
      bool isOverride, LambdaSymbol predicate, Maybe<Expression> _failure) : base(mutable, fieldName, expression, _typeConstraint, isHidden,
      isOverride)
   {
      this.predicate = predicate;
      this._failure = _failure;
   }

   public Maybe<TypeConstraint> TypeConstraint => _typeConstraint;

   public override void Generate(OperationsBuilder builder)
   {
      if (_failure is (true, var failure))
      {
         failure.Generate(builder);
      }

      predicate.Generate(builder);
      expression.Generate(builder);

      if (_typeConstraint is (true, var typeConstraint))
      {
         switch (typeConstraint.Comparisands[0].Name)
         {
            case "Optional":
               builder.ToOptional();
               break;
            case "Result":
               builder.ToResult();
               break;
         }
      }

      Module.Global.Value.ForwardReference(fieldName);
      builder.StoreGuardedField(fieldName, mutable, true, _typeConstraint, _failure);
   }
}