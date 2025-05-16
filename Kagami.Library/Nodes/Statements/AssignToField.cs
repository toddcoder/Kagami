using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using Core.Monads;
using static Core.Strings.StringStreamFunctions;

namespace Kagami.Library.Nodes.Statements;

public class AssignToField : Statement
{
   protected string fieldName;
   protected Maybe<Operation> _operation;
   protected Expression expression;

   public AssignToField(string fieldName, Maybe<Operation> _operation, Expression expression)
   {
      this.fieldName = fieldName;
      this._operation = _operation;
      this.expression = expression;
   }

   public override void Generate(OperationsBuilder builder)
   {
      if (_operation)
      {
         builder.GetField(fieldName);
      }

      expression.Generate(builder);

      if (_operation is(true, var operation))
      {
         builder.AddRaw(operation);
      }

      builder.Peek(Index);
      builder.AssignField(fieldName, false);
   }

   public override string ToString()
   {
      return stream() / fieldName / " " / (_operation.Map(o => o.ToString()) | (() => "")) / "= " / expression;
   }
}