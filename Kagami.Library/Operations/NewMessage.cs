using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class NewMessage : OneOperandOperation
{
   protected Selector selector;

   public NewMessage(Selector selector) => this.selector = selector;

   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      if (value is Arguments arguments)
      {
         return new Message(selector, arguments);
      }
      else
      {
         return incompatibleClasses(value, "Arguments");
      }
   }

   public override string ToString() => $"new.message({selector.AsString})";
}