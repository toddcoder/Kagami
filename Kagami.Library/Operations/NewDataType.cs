using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Collections;
using Standard.Types.Monads;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class NewDataType : OneOperandOperation
   {
      string className;

      public NewDataType(string className) => this.className = className;

      public override IMatched<IObject> Execute(Machine machine, IObject value)
      {
         if (value is Dictionary dictionary)
         {
            var hash = new Hash<string, (IObject[], IObject)>();
            foreach (var item in dictionary.InternalHash)
               if (item.Key is String name)
                  if (item.Value is Tuple tuple)
                  {
                     var dataComparisandName = name.Value;
                     if (tuple[0] is Tuple comparisands)
                     {
                        var ordinal = tuple[1];
                        hash[dataComparisandName] = (comparisands.Value, ordinal);
                     }
                     else
                        return failedMatch<IObject>(incompatibleClasses(tuple[0], "Tuple"));
                  }
                  else
                     return failedMatch<IObject>(incompatibleClasses(item.Value, "Tuple"));
               else
                  return failedMatch<IObject>(incompatibleClasses(item.Key, "String"));

            return new DataType(className, hash).Matched<IObject>();
         }
         else
            return failedMatch<IObject>(incompatibleClasses(value, "Dictionary"));
      }

      public override string ToString() => "new.data.type";
   }
}