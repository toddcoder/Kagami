using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using Core.Monads;
using static Core.Strings.StringStreamFunctions;

namespace Kagami.Library.Nodes.Statements
{
   public class AssignToField : Statement
   {
      protected string fieldName;
      protected IMaybe<Operation> _operation;
      protected Expression expression;

      public AssignToField(string fieldName, IMaybe<Operation> operation, Expression expression)
      {
         this.fieldName = fieldName;
         _operation = operation;
         this.expression = expression;
      }

      public override void Generate(OperationsBuilder builder)
      {
         if (_operation.IsSome)
         {
            builder.GetField(fieldName);
         }

         expression.Generate(builder);

         if (_operation.If(out var operation))
         {
            builder.AddRaw(operation);
         }

         builder.Peek(Index);
         builder.AssignField(fieldName, false);
      }

      public override string ToString()
      {
         return stream() / fieldName / " " / _operation.Map(o => o.ToString()).DefaultTo(() => "") / "= " / expression;
      }
   }
}