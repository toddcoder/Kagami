using System;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Collections;

namespace Kagami.Library.Packages
{
   public abstract class PackageClass : BaseClass
   {
      Hash<string, Func<IObject, Message, IObject>> functions;

      public PackageClass()
      {
         functions = new Hash<string, Func<IObject, Message, IObject>>();
      }

      protected void registerPackageFunction(string name, Func<IObject, Message, IObject> function)
      {
         registerMessage(name, function);
         functions[name] = function;
      }

      public void CopyToGlobalFrame(Package package)
      {
         var globalFrame = Machine.Current.GlobalFrame;
         var fields = globalFrame.Fields;

         foreach (var (functionName, func) in functions)
            if (!functionName.StartsWith("_") && !fields.ContainsKey(functionName))
               fields.New(functionName, new PackageFunction(package, functionName, func));

         foreach (var (fieldName, field) in package.Fields)
            if (!fieldName.StartsWith("_") && !fields.ContainsKey(fieldName))
               fields.New(fieldName, field);
      }
   }
}