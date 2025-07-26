using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Booleans;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class NewRange : TwoOperandOperation
{
   protected bool inclusive;

   public NewRange(bool inclusive) => this.inclusive = inclusive;

   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y) => x switch
   {
      Float start when y is Float stop => new FloatRange(start, stop, inclusive),
      IRangeItem start when y is Any => new KRange(start, new Infinity(true), inclusive),
      IRangeItem start when y is IObjectCompare stop => new KRange(start, stop, inclusive),
      KRange range when y is Int increment => new KRange(range, increment),
      FloatRange range when y is Float increment => new FloatRange(range, increment.Value),
      UserObject userObject when y is UserObject stop => new KRange(new UserRangeItem(userObject), new UserCompare(stop), inclusive),
      _ => incompatibleClasses(x, "RangeItem")
   };

   public override string ToString() => (StringStream)"new.range(" / inclusive.Extend("inclusive", "exclusive") / ")";
}