using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Standard.Types.Monads;
using static Standard.Types.Monads.MonadFunctions;
using static Standard.Types.Strings.StringStreamFunctions;

namespace Kagami.Library.Nodes.Statements
{
   public class AssignToNewField : Statement
   {
      bool mutable;
      string fieldName;
      Expression expression;
	   IMaybe<TypeConstraint> typeConstraint;

      public AssignToNewField(bool mutable, string fieldName, Expression expression, IMaybe<TypeConstraint> typeConstraint)
      {
         this.mutable = mutable;
         this.fieldName = fieldName;
         this.expression = expression;
	      this.typeConstraint = typeConstraint;
      }

	   public AssignToNewField(bool mutable, string fieldName, Expression expression)
	   {
		   this.mutable = mutable;
		   this.fieldName = fieldName;
		   this.expression = expression;
		   typeConstraint = none<TypeConstraint>();
	   }

      public bool Mutable => mutable;

      public string FieldName => fieldName;

      public override void Generate(OperationsBuilder builder)
      {
         builder.NewField(fieldName, mutable, true, typeConstraint);
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
}