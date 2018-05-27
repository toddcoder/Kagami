using Kagami.Library.Objects;
using Kagami.Library.Packages;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class OpenPackage : Operation
   {
      string packageName;

      public OpenPackage(string packageName) => this.packageName = packageName;

      public override IMatched<IObject> Execute(Machine machine)
      {
         var fieldName = packageName.ToLower1();
         if (machine.Find(fieldName, true).If(out var field, out var isNotMatched, out var exception))
            switch (field.Value)
            {
               case Package package when Module.Global.Class(package.ClassName).If(out var baseClass):
                  if (baseClass is PackageClass packageClass)
                  {
                     packageClass.RegisterMessages();
                     packageClass.CopyToGlobalFrame(package);
                     return notMatched<IObject>();
                  }
                  else
                     return failedMatch<IObject>(unableToConvert(baseClass.Name, "Package class"));
               case Package package:
                  return failedMatch<IObject>(classNotFound(package.ClassName));
               default:
                  return failedMatch<IObject>(unableToConvert(field.Value.Image, "Package"));
            }
         else if (isNotMatched)
            return failedMatch<IObject>(fieldNotFound(fieldName));
         else
            return failedMatch<IObject>(exception);
      }

      public override string ToString() => $"open.package({packageName})";
   }
}