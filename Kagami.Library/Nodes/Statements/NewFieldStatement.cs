using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class NewFieldStatement(string fieldName, bool mutable, Maybe<TypeConstraint> _typeConstraint) : Statement
{
   public override void Generate(OperationsBuilder builder) => builder.NewField(fieldName, mutable, _typeConstraint);
}