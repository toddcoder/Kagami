using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Booleans;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class IsClass : Operation
   {
      static IMatched<IObject> getValue(bool pop, Machine machine, string className)
      {
         if (pop)
            return machine.Pop().FlatMap(value => value.Matched(), failedMatch<IObject>);
         else
            return machine.Peek().FlatMap(value => value.Matched(), () => $"Couldn't peek value to determine class {className}".FailedMatch<IObject>());
      }

      string className;
      bool pop;

      public IsClass(string className, bool pop)
      {
         this.className = className;
         this.pop = pop;
      }

      public override IMatched<IObject> Execute(Machine machine)
      {
         return getValue(pop, machine, className).Map(value => Boolean.Object(value.ClassName == className));
      }

      public override string ToString() => $"is.class({className}{pop.Extend(", pop")})";
   }
}