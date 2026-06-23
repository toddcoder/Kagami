using Core.Monads;
using Kagami.Library.Objects;

namespace Kagami.Library.Nodes.Statements;

public interface IFieldStatement
{
   string Name { get; }

   bool Mutable { get; }

   Maybe<TypeConstraint> TypeConstraint { get; }
}