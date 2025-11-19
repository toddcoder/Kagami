using Core.Monads;
using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public interface ICollectionType
{
   public Maybe<TypeConstraint> SubTypeConstraint { get; set; }
}