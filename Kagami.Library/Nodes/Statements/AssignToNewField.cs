using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Core.Monads;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Core.Strings.StringStreamFunctions;

namespace Kagami.Library.Nodes.Statements;

public class AssignToNewField : Statement
{
   protected readonly bool mutable;
   protected readonly string fieldName;
   protected readonly bool tolerant;
   protected readonly Expression expression;
   protected readonly Maybe<TypeConstraint> _typeConstraint;

   public AssignToNewField(bool mutable, string fieldName, bool tolerant, Expression expression, Maybe<TypeConstraint> _typeConstraint)
   {
      this.mutable = mutable;
      this.fieldName = fieldName;
      this.tolerant = tolerant;
      this.expression = expression;
      this._typeConstraint = _typeConstraint;
   }

   public AssignToNewField(bool mutable, string fieldName, bool tolerant, Expression expression)
   {
      this.mutable = mutable;
      this.fieldName = fieldName;
      this.tolerant = tolerant;
      this.expression = expression;

      _typeConstraint = nil;
   }

   public bool Mutable => mutable;

   public string FieldName => fieldName;

   public override void Generate(OperationsBuilder builder)
   {
      expression.Generate(builder);
      Module.Global.Value.ForwardReference(fieldName);
      builder.StoreField(fieldName, mutable, true, _typeConstraint);
   }

   public override string ToString() => stream() / (mutable ? "var" : "let") / " " / fieldName / " = " / expression;

   public void Deconstruct(out bool mutable, out string fieldName, out Maybe<TypeConstraint> _typeConstraint)
   {
      mutable = this.mutable;
      fieldName = this.fieldName;
      _typeConstraint = this._typeConstraint;
   }

   public bool Ignore { get; set; }
}