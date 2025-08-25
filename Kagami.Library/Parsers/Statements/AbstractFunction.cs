using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;

namespace Kagami.Library.Parsers.Statements;

public record AbstractFunction(Selector Selector, Parameters Parameters, Maybe<TypeConstraint> TypeConstraint);