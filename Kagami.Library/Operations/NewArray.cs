using System.Linq;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations
{
   public class NewArray : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value) => value switch
      {
         Container list => Array.CreateObject(list.List.ToArray()).Matched(),
         IKeyValue => Array.CreateObject(new[] { value }).Matched(),
         ICollection { ExpandForArray: true } collection => Array.CreateObject(collection.GetIterator(false).List().ToArray()).Matched(),
         IIterator iterator => Array.CreateObject(iterator.List().ToArray()).Matched(),
         _ => new Array(value).Matched<IObject>()
      };

      public override string ToString() => "new.array";
   }
}