using System;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Collections;

namespace Kagami.Library.Packages
{
   public abstract class PackageClass : BaseClass
   {
      SelectorHash<Func<IObject, Message, IObject>> functions;

      public PackageClass()
      {
         functions = new SelectorHash<Func<IObject, Message, IObject>>();
      }

      protected void registerPackageFunction(Selector selector, Func<IObject, Message, IObject> function)
      {
         registerMessage(selector, function);
         functions[selector] = function;
      }

      public void CopyToGlobalFrame(Package package)
      {
         var globalFrame = Machine.Current.GlobalFrame;
         var fields = globalFrame.Fields;

         foreach (var (functionName, func) in functions)
         {
	         Selector selector = functionName;
	         if (!functionName.StartsWith("_") && !fields.ContainsKey(selector))
		         fields.New(selector, new PackageFunction(package, functionName, func));
         }

         foreach (var (fieldName, field) in package.Fields)
	         if (!fieldName.StartsWith("_") && !fields.ContainsKey(fieldName))
		         fields.New(fieldName, field);
      }
   }
}