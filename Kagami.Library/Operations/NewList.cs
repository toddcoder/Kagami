using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations
{
   public class NewList : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value) => value switch
      {
         Container list => List.NewList(list).Matched<IObject>(),
         ICollection { ExpandForArray: true } collection => List.NewList(list(collection)).Matched<IObject>(),
         IIterator iterator => List.NewList(iterator.List()).Matched<IObject>(),
         _ => List.Single(value).Matched<IObject>()
      };

      public override string ToString() => "new.list";
   }
}