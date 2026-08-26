using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Parsers.Statements;

namespace Kagami.Library.Nodes.Statements;

public abstract class BuilderStatement(BuilderState builderState) : Statement
{
   protected BuilderState builderState = builderState;

   public void Prefix(OperationsBuilder builder)
   {
      if (!builderState.First)
      {
         builder.GetField(builderState.ResultFieldName);
         builder.GoToIfFalse(builderState.FailureLabel, false);
      }
   }

   public void Assign(OperationsBuilder builder, IObject value)
   {
      builder.PushObject(value);
      builder.Success();
      Assign(builder);
   }

   public void Assign(OperationsBuilder builder) => builder.AssignField(builderState.ResultFieldName, false);
}