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
   protected readonly Expression expression;
   protected readonly Maybe<TypeConstraint> _typeConstraint;
   protected readonly bool isHidden;

   public AssignToNewField(bool mutable, string fieldName, Expression expression, Maybe<TypeConstraint> _typeConstraint, bool isHidden)
   {
      this.mutable = mutable;
      this.fieldName = fieldName;
      this.expression = expression;
      this._typeConstraint = _typeConstraint;
      this.isHidden = isHidden;
   }

   public AssignToNewField(bool mutable, string fieldName, Expression expression, bool isHidden)
   {
      this.mutable = mutable;
      this.fieldName = fieldName;
      this.expression = expression;
      this.isHidden = isHidden;

      _typeConstraint = nil;
   }

   public bool Mutable => mutable;

   public string FieldName => fieldName;

   public bool IsHidden => isHidden;

   public override void Generate(OperationsBuilder builder)
   {
      expression.Generate(builder);
      Module.Global.Value.ForwardReference(fieldName);
      builder.StoreField(fieldName, mutable, true, _typeConstraint);
   }

   public override string ToString() => stream() / (mutable ? "var" : "let") / " " / fieldName / " = " / expression;

   public void Deconstruct(out bool mutable, out string fieldName, out Maybe<TypeConstraint> _typeConstraint, out bool isHidden)
   {
      mutable = this.mutable;
      fieldName = this.fieldName;
      _typeConstraint = this._typeConstraint;
      isHidden = this.isHidden;
   }

   public bool Ignore { get; set; }
}