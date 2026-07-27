using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Core.Monads;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Core.Strings.StringStreamFunctions;

namespace Kagami.Library.Nodes.Statements;

public class AssignToNewField : Statement, IFieldStatement
{
   protected readonly bool mutable;
   protected readonly string fieldName;
   protected readonly Expression expression;
   protected readonly Maybe<TypeConstraint> _typeConstraint;
   protected readonly bool isHidden;
   protected readonly bool isOverride;

   public AssignToNewField(bool mutable, string fieldName, Expression expression, Maybe<TypeConstraint> _typeConstraint, bool isHidden,
      bool isOverride)
   {
      this.mutable = mutable;
      this.fieldName = fieldName;
      this.expression = expression;
      this._typeConstraint = _typeConstraint;
      this.isHidden = isHidden;
      this.isOverride = isOverride;
   }

   public AssignToNewField(bool mutable, string fieldName, Expression expression, bool isHidden, bool isOverride)
   {
      this.mutable = mutable;
      this.fieldName = fieldName;
      this.expression = expression;
      this.isHidden = isHidden;
      this.isOverride = isOverride;

      _typeConstraint = nil;
   }

   public bool Mutable => mutable;

   public string FieldName => fieldName;

   public bool IsHidden => isHidden;

   public bool IsOverride => isOverride;

   public override void Generate(OperationsBuilder builder)
   {
      expression.Generate(builder);

      if (_typeConstraint is (true, var typeConstraint and not ProtocolConstraint))
      {
         switch (typeConstraint.Comparisands[0].Name)
         {
            case "Optional":
               builder.ToOptional();
               break;
            case "Result":
               builder.ToResult();
               break;
         }
      }

      Module.Global.Value.ForwardReference(fieldName);
      builder.StoreField(fieldName, mutable, true, isOverride, _typeConstraint);
   }

   public override string ToString() => stream() / (mutable ? "var" : "let") / " " / fieldName / " = " / expression;

   public void Deconstruct(out bool mutable, out string fieldName, out Maybe<TypeConstraint> _typeConstraint, out bool isHidden, out bool isOverride,
      out Expression expression)
   {
      mutable = this.mutable;
      fieldName = this.fieldName;
      _typeConstraint = this._typeConstraint;
      isHidden = this.isHidden;
      isOverride = this.isOverride;
      expression = this.expression;
   }

   public bool Ignore { get; set; }

   public string Name => fieldName;

   public Maybe<TypeConstraint> TypeConstraint => _typeConstraint;
}