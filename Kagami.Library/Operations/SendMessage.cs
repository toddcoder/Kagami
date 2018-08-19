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
      Selector selector;

      public SendMessage(Selector selector) => this.selector = selector;

      public override string ToString() => $"send.message({selector.Image})";

      public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y)
      {
         try
         {
            switch (y)
            {
               case Arguments arguments when x is Class:
                  return classOf(x).SendClassMessage(selector, arguments).Matched();
               case Arguments arguments:
                  return classOf(x).SendMessage(x, selector, arguments).Matched();
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