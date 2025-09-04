using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Packages;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class JunctionInvoke(string functionName) : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      if (value is Arguments arguments)
      {
         var selector = arguments.Selector(functionName);
         var _field = machine.Find(selector);
         if (_field is (true, var field))
         {
            switch (field.Value)
            {
               case Lambda lambda:
               {
                  lambda.Capture(machine);

                  List<IObject> results = [];
                  foreach (var junctionArguments in arguments.ExpandJunctions())
                  {
                     var result = lambda.Invoke(junctionArguments.Value);
                     results.Add(result);
                  }

                  var junction = new Junction(JunctionType.Any, results);
                  return junction.Flatten();
               }
               case PackageFunction packageFunction:
               {
                  List<IObject> results = [];
                  foreach (var junctionArguments in arguments.ExpandJunctions())
                  {
                     var result = packageFunction.Invoke(junctionArguments.Value);
                     results.Add(result);
                  }

                  var junction = new Junction(JunctionType.Any, results);
                  return junction.Flatten();
               }
               default:
                  return incompatibleClasses(field.Value, "Lambda or PackageFunction");
            }
         }
         else
         {
            return fieldNotFound(selector);
         }
      }
      else
      {
         return incompatibleClasses(value, "Arguments");
      }
   }

   public override string ToString() => "junction.invoke";
}