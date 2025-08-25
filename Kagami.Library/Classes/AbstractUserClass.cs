using Core.Collections;
using Kagami.Library.Objects;
using Kagami.Library.Parsers.Statements;

namespace Kagami.Library.Classes;

public class AbstractUserClass : UserClass
{
   protected Hash<Selector, AbstractFunction> abstractFunctions;

   public AbstractUserClass(string className, string parentClassName, Hash<Selector, AbstractFunction> abstractFunctions) : base(className,
      parentClassName)
   {
      this.abstractFunctions = abstractFunctions;
   }
}