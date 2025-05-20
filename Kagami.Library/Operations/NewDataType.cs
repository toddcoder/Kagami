using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Monads;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class NewDataType : OneOperandOperation
{
   protected string className;

   public NewDataType(string className) => this.className = className;

   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      if (value is Dictionary dictionary)
      {
         var hash = new Hash<string, (IObject[], IObject)>();
         foreach (var (key, objectValue) in dictionary.InternalHash)
         {
            if (key is KString name)
            {
               if (objectValue is KTuple tuple)
               {
                  var dataComparisandName = name.Value;
                  if (tuple[0] is KTuple comparisands)
                  {
                     var ordinal = tuple[1];
                     hash[dataComparisandName] = (comparisands.Value, ordinal);
                  }
                  else
                  {
                     return incompatibleClasses(tuple[0], "Tuple");
                  }
               }
               else
               {
                  return incompatibleClasses(objectValue, "Tuple");
               }
            }
            else
            {
               return incompatibleClasses(key, "String");
            }
         }

         return new DataType(className, hash);
      }
      else
      {
         return incompatibleClasses(value, "Dictionary");
      }
   }

   public override string ToString() => "new.data.type";
}