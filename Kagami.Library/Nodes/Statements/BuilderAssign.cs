using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using Kagami.Library.Parsers.Statements;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Nodes.Statements;

public class BuilderAssign(BuilderState state, string fieldName, Expression expression, bool first) : Statement
{
   public override void Generate(OperationsBuilder builder)
   {
      if (!first)
      {
         builder.GetField(state.ResultFieldName);
         builder.GoToIfFalse(state.FailureLabel);
      }

      expression.Generate(builder);
      builder.Dup();
      builder.AssignField(state.ResultFieldName, false);
      builder.UnwrapMonad();
      builder.StoreField(fieldName, false, false, true, nil);
   }

   public override string ToString() => $"let {fieldName} = {expression} [builder]";
}