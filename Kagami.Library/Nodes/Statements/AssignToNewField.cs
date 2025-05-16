using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Core.Strings.StringStreamFunctions;

namespace Kagami.Library.Nodes.Statements;

public class AssignToNewField : Statement
{
   protected bool mutable;
   protected string fieldName;
   protected Expression expression;
   protected Maybe<TypeConstraint> _typeConstraint;

   public AssignToNewField(bool mutable, string fieldName, Expression expression, Maybe<TypeConstraint> _typeConstraint)
   {
      this.mutable = mutable;
      this.fieldName = fieldName;
      this.expression = expression;
      this._typeConstraint = _typeConstraint;
   }

   public AssignToNewField(bool mutable, string fieldName, Expression expression)
   {
      this.mutable = mutable;
      this.fieldName = fieldName;
      this.expression = expression;

      _typeConstraint = nil;
   }

   public bool Mutable => mutable;

   public string FieldName => fieldName;

   public override void Generate(OperationsBuilder builder)
   {
      builder.NewField(fieldName, mutable, true, _typeConstraint);
      expression.Generate(builder);
      builder.Peek(Index);
      builder.AssignField(fieldName, false);

      builder.GetField(fieldName);
   }

   public override string ToString() => stream() / (mutable ? "var" : "let") / " " / fieldName / " = " / expression;

   public void Deconstruct(out bool mutable, out string fieldName, out Expression expression)
   {
      mutable = this.mutable;
      fieldName = this.fieldName;
      expression = this.expression;
   }
}