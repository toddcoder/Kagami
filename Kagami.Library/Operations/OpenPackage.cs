using Kagami.Library.Objects;
using Kagami.Library.Packages;
using Kagami.Library.Runtime;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class OpenPackage : Operation
{
   protected string packageName;

   public OpenPackage(string packageName) => this.packageName = packageName;

   public override Optional<IObject> Execute(Machine machine)
   {
      var fieldName = packageName.ToLower1();
      var _field = machine.Find(fieldName, true);
      if (_field is (true, var field))
      {
         switch (field.Value)
         {
            case Package package when Module.Global.Class(package.ClassName) is (true, var baseClass):
            {
               if (baseClass is PackageClass packageClass)
               {
                  packageClass.RegisterMessages();
                  packageClass.CopyToGlobalFrame(package);

                  return nil;
               }
               else
               {
                  return unableToConvert(baseClass.Name, "Package class");
               }
            }

            case Package package:
               return classNotFound(package.ClassName);
            default:
               return unableToConvert(field.Value.Image, "Package");
         }
      }
      else if (_field.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return fieldNotFound(fieldName);
      }
   }

   public override string ToString() => $"open.package({packageName})";
}