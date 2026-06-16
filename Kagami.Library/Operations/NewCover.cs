using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class NewCover : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => value switch
   {
      ICollection collection => new Cover(collection),
      IIterator iterator => new Cover(iterator.Collection),
      _ => fail("Cannot create a cover from the given value")
   };

   public override string ToString() => "new.cover";
}