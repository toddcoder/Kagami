using Kagami.Library.Objects;
using Core.Monads;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class NewTuple : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => value switch
   {
      Sequence sequence => new KTuple([.. sequence.List]),
      ICollection { ExpandForArray: true } collection => new KTuple([.. collection.GetIterator(false).List()]),
      IIterator iterator => new KTuple([.. iterator.List()]),
      _ => new KTuple([value])
   };

   public override string ToString() => "new.tuple";
}