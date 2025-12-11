using Core.Monads;
using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public interface ITypedCollection
{
   Maybe<TypeConstraint> TypeConstraint { get; set; }

   IObject SetType(TypeConstraint typeConstraint);

   IObject AutoType();
}