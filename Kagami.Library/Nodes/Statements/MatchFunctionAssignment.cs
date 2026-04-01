using Kagami.Library.Invokables;
using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Nodes.Statements;

public class MatchFunctionAssignment(string fieldName, Parameters parameters) : Statement
{
   public override void Generate(OperationsBuilder builder)
   {
      switch (parameters.Length)
      {
         case 1:
            builder.GetField(parameters[0].Name);
            builder.StoreField(fieldName, false, true, parameters[0].TypeConstraint);
            break;
         case >= 2:
         {
            var field0 = parameters[0].Name;
            var field1 = parameters[1].Name;
            builder.GetField(field0);
            builder.GetField(field1);
            builder.NewSequence();

            for (var i = 2; i < parameters.Length; i++)
            {
               builder.GetField(parameters[i].Name);
               builder.NewSequence();
            }

            builder.NewTuple();
            builder.StoreField(fieldName, false, true, nil);
            break;
         }
      }
   }
}