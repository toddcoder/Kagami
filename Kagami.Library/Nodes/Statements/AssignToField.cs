using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using Standard.Types.Maybe;
using static Standard.Types.Strings.StringStreamFunctions;

namespace Kagami.Library.Nodes.Statements
{
   public class AssignToField : Statement
   {
      string fieldName;
      IMaybe<Operation> operation;
      Expression expression;

      public AssignToField(string fieldName, IMaybe<Operation> operation, Expression expression)
      {
         this.fieldName = fieldName;
         this.operation = operation;
         this.expression = expression;
      }

      public override void Generate(OperationsBuilder builder)
      {
         if (operation.If(out var op))
            builder.GetField(fieldName);

         expression.Generate(builder);

         if (operation.If(out op))
            builder.AddRaw(op);

         builder.Peek(Index);
         builder.AssignField(fieldName, false);
      }

      public override string ToString()
      {
         return stream() / fieldName / " " / operation.FlatMap(o => o.ToString(), () => "") / "= " / expression;
      }
   }
}