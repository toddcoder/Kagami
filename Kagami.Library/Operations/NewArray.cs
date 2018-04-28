using System.Linq;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
   public class NewArray : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value)
      {
         switch (value)
         {
            case InternalList list:
               return Array.CreateObject(list.List.ToArray()).Matched();
            case IKeyValue _:
               return Array.CreateObject(new[] { value }).Matched();
            case ICollection collection when collection.ExpandForArray:
               return Array.CreateObject(collection.GetIterator(false).List().ToArray()).Matched();
            case IIterator iterator:
               return Array.CreateObject(iterator.List().ToArray()).Matched();
            default:
               return new Array(value).Matched<IObject>();
         }
      }

      public override string ToString() => "new.array";
   }
}