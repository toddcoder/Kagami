using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using static Standard.Types.Strings.StringStreamFunctions;

namespace Kagami.Library.Nodes.Statements
{
   public class AssignToNewField : Statement
   {
      bool mutable;
      string fieldName;
      Expression expression;

      public AssignToNewField(bool mutable, string fieldName, Expression expression)
      {
         this.mutable = mutable;
         this.fieldName = fieldName;
         this.expression = expression;
      }

      public bool Mutable => mutable;

      public string FieldName => fieldName;

      public override void Generate(OperationsBuilder builder)
      {
         builder.NewField(fieldName, mutable, true);
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