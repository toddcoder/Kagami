using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Booleans;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class IsClass : Operation
   {
      protected static IMatched<IObject> getValue(bool pop, Machine machine, string className)
      {
         if (pop)
         {
            return machine.Pop().Map(value => value.Matched()).Recover(failedMatch<IObject>);
         }
         else
         {
            return machine.Peek().Map(value => value.Matched())
               .DefaultTo(() => $"Couldn't peek value to determine class {className}".FailedMatch<IObject>());
         }
      }

      protected string className;
      protected bool pop;

      public IsClass(string className, bool pop)
      {
         this.className = className;
         this.pop = pop;
      }

      public override IMatched<IObject> Execute(Machine machine)
      {
         return getValue(pop, machine, className).Map(value => Boolean.BooleanObject(value.ClassName == className));
      }

      public override string ToString() => $"is.class({className}{pop.Extend(", pop")})";
   }
}