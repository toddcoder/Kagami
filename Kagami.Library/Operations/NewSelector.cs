using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class NewSelector : Operation
{
   protected Selector selector;
   protected bool mutable;
   protected bool visible;

   public NewSelector(Selector selector, bool mutable, bool visible)
   {
      this.selector = selector;
      this.mutable = mutable;
      this.visible = visible;
   }

   public override Optional<IObject> Execute(Machine machine)
   {
      var _fields = machine.CurrentFrame.Fields.New(selector, mutable, visible);
      if (_fields)
      {
         return nil;
      }
      else
      {
         return _fields.Exception;
      }
   }

   public override string ToString() => $"new.selector({selector}, {mutable}, {visible})";
}