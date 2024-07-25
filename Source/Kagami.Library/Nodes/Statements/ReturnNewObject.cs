using Kagami.Library.Invokables;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements
{
   public class ReturnNewObject : Statement
   {
      protected string className;
      protected Parameters parameters;

      public ReturnNewObject(string className, Parameters parameters)
      {
         this.className = className;
         this.parameters = parameters;
      }

      public override void Generate(OperationsBuilder builder)
      {
         builder.NewObject(className, parameters);
         builder.Return(true);
      }
   }
}