using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class NewField : Operation
   {
      string name;
      bool mutable;
      bool visible;

      public NewField(string name, bool mutable, bool visible)
      {
         this.name = name;
         this.mutable = mutable;
         this.visible = visible;
      }

      public override IMatched<IObject> Execute(Machine machine)
      {
         if (machine.CurrentFrame.Fields.New(name, mutable, visible).If(out _, out var exception))
            return notMatched<IObject>();
         else
            return failedMatch<IObject>(exception);
      }

      public override string ToString() => $"new.field({name}, {mutable.ToString().ToLower()}, {visible.ToString().ToLower()})";
   }
}