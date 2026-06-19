using Core.Monads;
using Kagami.Library.Objects;

namespace Kagami.Library.Protocols;

public record SelectorWithType(Selector Selector, Maybe<TypeConstraint> TypeConstraint);