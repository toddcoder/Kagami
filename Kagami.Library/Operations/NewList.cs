using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations
{
   public class NewList : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value)
      {
         switch (value)
         {
            case InternalList list:
               return List.NewList(list).Matched<IObject>();
            case ICollection collection when collection.ExpandForArray:
               return List.NewList(list(collection)).Matched<IObject>();
            case IIterator iterator:
               return List.NewList(iterator.List()).Matched<IObject>();
            default:
               return List.Single(value).Matched<IObject>();
         }
      }

      public override string ToString() => "new.list";
   }
}