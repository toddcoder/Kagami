using System;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
   public class NewValue : ArgumentsOperation
   {
      string className;
      Func<Arguments, IObject> initializer;

      public NewValue(string className, Func<Arguments, IObject> initializer)
      {
         this.className = className;
         this.initializer = initializer;
      }

      public override string ToString() => $"new.value({className})";

      public override IMatched<IObject> Execute(Machine machine, Arguments arguments) => initializer(arguments).Matched();
   }
}