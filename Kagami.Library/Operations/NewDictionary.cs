using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class NewDictionary : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => value switch
   {
      Sequence list => Dictionary.New(list.List.ToArray()).Just(),
      KArray array => Dictionary.New(array.List.ToArray()).Just(),
      KTuple tuple => Dictionary.New(tuple.List.ToArray()).Just(),
      IKeyValue => Dictionary.New([value]).Just(),
      ICollection { ExpandForArray: true } collection => Dictionary.New(collection.GetIterator(false).List().ToArray()).Just(),
      IIterator iterator => Dictionary.New(iterator.List().ToArray()).Just(),
      _ => fail($"Dictionary can't be created with {value}")
   };
}