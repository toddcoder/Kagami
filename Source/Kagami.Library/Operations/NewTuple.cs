using Kagami.Library.Objects;
using Core.Monads;

namespace Kagami.Library.Operations
{
   public class NewTuple : InternalListOperation
   {
      public override IMatched<IObject> Execute(Container list)
      {
         return new Tuple(list.List.ToArray()).Matched<IObject>();
      }

      public override string ToString() => "new.tuple";
   }
}