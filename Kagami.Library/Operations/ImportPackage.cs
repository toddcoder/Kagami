using System;
using System.Reflection;
using Kagami.Library.Objects;
using Kagami.Library.Packages;
using Kagami.Library.Runtime;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;
using Module = Kagami.Library.Runtime.Module;

namespace Kagami.Library.Operations
{
   public class ImportPackage : Operation
   {
      static AutoHash<string, Assembly> assemblyCache;

      static ImportPackage()
      {
         assemblyCache = new AutoHash<string, Assembly>(packageName =>
         {
            var upperPackageName = packageName.ToUpper1();
            var fullPath = System.IO.Path.Combine(Machine.Current.PackageFolder, $"Kagami.{upperPackageName}.dll");
            var assembly = Assembly.LoadFile(fullPath);
            return assembly;
         }, true);
      }

      string packageName;

      public ImportPackage(string packageName) => this.packageName = packageName;

      public override IMatched<IObject> Execute(Machine machine)
      {
         var upperPackageName = packageName.ToUpper1();
         var assembly = assemblyCache[packageName];
         var type = assembly.GetType($"Kagami.{upperPackageName}.{upperPackageName}");
         if (type is null)
            return failedMatch<IObject>(classNotFound(upperPackageName));
         else
         {
            var package = (Package)Activator.CreateInstance(type, null);
            package.LoadTypes(Module.Global);
            var globalFrame = machine.GlobalFrame;
            var fields = globalFrame.Fields;
            fields.New(packageName, package);

            return notMatched<IObject>();
         }
      }

      public override string ToString() => $"import.package({packageName})";
   }
}