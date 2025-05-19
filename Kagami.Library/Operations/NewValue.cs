using System;
using Kagami.Library.Objects;
using Core.Monads;

namespace Kagami.Library.Operations;

public class NewValue : ArgumentsOperation
{
   protected string className;
   protected Func<Arguments, IObject> initializer;

   public NewValue(string className, Func<Arguments, IObject> initializer)
   {
      this.className = className;
      this.initializer = initializer;
   }

   public override string ToString() => $"new.value({className})";

   public override Optional<IObject> Execute(Arguments arguments) => initializer(arguments).Just();
}