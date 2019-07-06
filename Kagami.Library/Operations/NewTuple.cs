using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations
{
   public class NewTuple : InternalListOperation
   {
      public override IMatched<IObject> Execute(Machine machine, Container list)
      {
         return new Tuple(list.List.ToArray()).Matched<IObject>();
      }

      public override string ToString() => "new.tuple";
   }
}