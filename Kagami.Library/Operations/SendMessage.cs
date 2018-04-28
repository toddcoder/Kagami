using System;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class SendMessage : TwoOperandOperation
   {
      string message;

      public SendMessage(string message) => this.message = message;

      public override string ToString() => $"send.message({message})";

      public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y)
      {
         try
         {
            switch (y)
            {
               case Arguments arguments when x is Class:
                  return classOf(x).SendClassMessage(arguments.FullFunctionName(message), arguments).Matched();
               case Arguments arguments:
                  return classOf(x).SendMessage(x, arguments.FullFunctionName(message), arguments).Matched();
               default:
                  return failedMatch<IObject>(incompatibleClasses(y, "Arguments"));
            }
         }
         catch (Exception exception)
         {
            return failedMatch<IObject>(exception);
         }
      }
   }
}