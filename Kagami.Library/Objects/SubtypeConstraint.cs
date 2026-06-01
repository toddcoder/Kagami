using Core.Monads;
using Kagami.Library.Classes;
using Kagami.Library.Nodes.Symbols;

namespace Kagami.Library.Objects;

public class SubtypeConstraint(string subtypeName) : TypeConstraint([])
{
   protected Lazy<(LambdaSymbol lambdaSymbol, Maybe<TypeConstraint> typeConstraint, Maybe<Expression> failure)> subtype = new(() => Guards.Subtype.GetOrThrow(subtypeName));

   public (LambdaSymbol lambdaSymbol, Maybe<TypeConstraint> typeConstraint, Maybe<Expression> failure) Subtype => subtype.Value;

   public override int Hash => subtype.Value.GetHashCode();

   public override bool IsEqualTo(IObject obj) => obj is SubtypeConstraint subtypeConstraint && subtype.Value.Equals(subtypeConstraint.subtype.Value);

   public override bool Matches(BaseClass baseClass) => subtype.Value.typeConstraint.Map(tc => tc.Matches(baseClass)) | true;

   public override bool IsEquivalentTo(TypeConstraint typeConstraint)
   {
      if (subtype.Value.typeConstraint is (true, var subtypeTypeConstraint))
      {
         return typeConstraint.IsEquivalentTo(subtypeTypeConstraint);
      }
      else
      {
         return true;
      }
   }

   public override string AsString => subtypeName;

   public override string Image => subtypeName;
}