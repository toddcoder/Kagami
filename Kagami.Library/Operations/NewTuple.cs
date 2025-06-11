using Kagami.Library.Objects;
using Core.Monads;

namespace Kagami.Library.Operations;

public class NewTuple : InternalListOperation
{
   public override Optional<IObject> Execute(Sequence list)
   {
      return new KTuple(list.List.ToArray());
   }

   public override string ToString() => "new.tuple";
}