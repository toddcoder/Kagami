using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class NewDictionaryOrSet : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      if (value is KeyValue)
      {
         return Dictionary.New([value]).Just();
      }

      var _list = value switch
      {
         Sequence sequence => sequence.List,
         KArray array => array.List.Some(),
         KTuple tuple => tuple.List.Some(),
         ICollection { ExpandForArray: true } collection => collection.GetIterator(false).List().Some(),
         Iterator iterator => iterator.List().Some(),
         _ => nil
      };
      if (_list is (true, var list))
      {
         IObject[] objects = [.. list];
         if (objects.All(o => o is KeyValue or NameValue))
         {
            return new Dictionary(objects);
         }
         else
         {
            return new Set(objects);
         }
      }
      else
      {
         return fail($"Dictionary or Set can't be created with {value.AsString}");
      }
   }

   public override string ToString() => "new.dictionary.or.set";
}