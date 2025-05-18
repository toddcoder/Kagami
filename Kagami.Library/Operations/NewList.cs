using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class NewList : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => value switch
   {
      Container list => List.NewList(list),
      ICollection { ExpandForArray: true } collection => List.NewList(list(collection)),
      IIterator iterator => List.NewList(iterator.List()),
      _ => List.Single(value)
   };

   public override string ToString() => "new.list";
}