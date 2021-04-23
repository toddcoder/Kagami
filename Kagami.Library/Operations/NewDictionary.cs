using System.Linq;
using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations
{
   public class NewDictionary : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value) => value switch
      {
         Container list => Dictionary.New(list.List.ToArray()).Matched(),
         IKeyValue => Dictionary.New(new[] { value }).Matched(),
         ICollection { ExpandForArray: true } collection => Dictionary.New(collection.GetIterator(false).List().ToArray()).Matched(),
         IIterator iterator => Dictionary.New(iterator.List().ToArray()).Matched(),
         _ => $"Dictionary can't be created with {value}".FailedMatch<IObject>()
      };
   }
}