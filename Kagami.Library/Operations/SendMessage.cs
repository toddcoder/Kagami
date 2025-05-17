using System;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class SendMessage : TwoOperandOperation
{
   protected Selector selector;

   public SendMessage(Selector selector) => this.selector = selector;

   public override string ToString() => $"send.message({selector.Image})";

   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      try
      {
         return y switch
         {
            Arguments arguments when x is Class => classOf(x).SendClassMessage(selector, arguments).Just(),
            Arguments arguments => classOf(x).SendMessage(x, selector, arguments).Just(),
            _ => incompatibleClasses(y, "Arguments")
         };
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}