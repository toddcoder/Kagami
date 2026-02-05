using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class NewDictionaryOrSet(Maybe<TypeConstraint> _typeConstraint) : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      switch (value)
      {
         case KeyValue:
         {
            return Dictionary.New([value], _typeConstraint).Just();
         }
         case KArray:
            return value.Just();
         case Junction junction:
            return new Set(junction.Items);
      }

      var _list = value switch
      {
         Sequence sequence => sequence.List,
         KArray array => array.List.Some(),
         KTuple tuple => tuple.List.Some(),
         ICollection { ExpandForArray: true } collection => collection.GetIterator(false).List().Some(),
         IIterator iterator => iterator.List().Some(),
         NameValue => new List<IObject> { value },
         _ => nil
      };
      if (_list is (true, var list))
      {
         IObject[] objects = [.. list];
         if (objects.All(o => o is KeyValue or NameValue))
         {
            return new Dictionary(objects) { TypeConstraint = _typeConstraint };
         }
         else
         {
            return new Set(objects) { TypeConstraint = _typeConstraint };
         }
      }
      else
      {
         return new Set(value) { TypeConstraint = _typeConstraint };
      }
   }

   public override string ToString() => "new.dictionary.or.set";
}