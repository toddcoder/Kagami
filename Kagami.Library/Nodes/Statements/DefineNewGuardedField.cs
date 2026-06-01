using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Runtime;

namespace Kagami.Library.Nodes.Statements;

public class DefineNewGuardedField(bool mutable, string fieldName, TypeConstraint typeConstraint, bool isHidden, bool isOverride,
   LambdaSymbol predicate, Maybe<Expression> failure) : DefineNewField(mutable, fieldName, typeConstraint, isHidden, isOverride, false)
{
   public override void Generate(OperationsBuilder builder)
   {
      predicate.Generate(builder);
      builder.PushObject(typeConstraint.Comparisands[0].DefaultValue);

      switch (typeConstraint.Comparisands[0].Name)
      {
         case "Optional":
            builder.ToOptional();
            break;
         case "Result":
            builder.ToResult();
            break;
      }

      Module.Global.Value.ForwardReference(fieldName);
      builder.StoreGuardedField(fieldName, mutable, true, typeConstraint, failure);
   }
}