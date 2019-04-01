using System;
using System.Reflection;
using Kagami.Library.Objects;
using Kagami.Library.Packages;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;
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
            var fullPath = System.IO.Path.Combine(Machine.Current.PackageFolder, $"Kagami.{packageName}.dll");
            var assembly = Assembly.LoadFile(fullPath);
            return assembly;
         }, true);
      }

      string packageName;

      public ImportPackage(string packageName) => this.packageName = packageName;

      public override IMatched<IObject> Execute(Machine machine)
      {
         var ns = packageName.ToUpper1();
         var packageClassName = ns.Copy();
         var assemblyName = ns.Copy();
         switch (ns)
         {
            case "Io":
               ns = "IO";
               packageClassName = "IO";
               assemblyName = "IO";
               break;
         }

         var assembly = assemblyCache[assemblyName];
         var type = assembly.GetType($"Kagami.{ns}.{packageClassName}");
         if (type is null)
            return failedMatch<IObject>(classNotFound(ns));
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