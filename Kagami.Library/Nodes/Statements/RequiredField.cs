using Core.Enumerables;
using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Nodes.Statements;

public class RequiredField(string fieldName, bool mutable, Maybe<TypeConstraint> _typeConstraint) : Statement
{
   public string FieldName => fieldName;

   public bool Mutable => mutable;

   public Maybe<TypeConstraint> TypeConstraint => _typeConstraint;

   public bool Matches(Parameter parameter) => matchingTypeConstraints(parameter.TypeConstraint, _typeConstraint) && parameter.Mutable == mutable;

   public override void Generate(OperationsBuilder builder)
   {
   }

   public override string ToString()
   {
      var varLet = mutable ? "var" : "let";
      var typeConstraintImage = _typeConstraint.Map(t => t.Comparisands.Select(c => c.Name).ToString("|")) | "";
      return $"required {varLet} {fieldName} {typeConstraintImage}";
   }
}