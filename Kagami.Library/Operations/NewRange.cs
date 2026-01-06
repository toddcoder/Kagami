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
   protected int increment;

   public NewRange(bool inclusive, bool down)
   {
      this.inclusive = inclusive;
      increment = down ? -1 : 1;
   }

   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y) => x switch
   {
      Float start when y is Float stop => new FloatRange(start, stop, inclusive, increment),
      Long start when y is Long stop => new LongRange(start, stop, inclusive, increment),
      IRangeItem start when y is Any => new KRange(start, new Infinity(true), inclusive, increment),
      IRangeItem start when y is IObjectCompare stop => new KRange(start, stop, inclusive, increment),
      IRangeItem start when y is IRangeItem stop => new KRange(start, stop, inclusive, increment),
      KRange range when y is Int intIncrement => new KRange(range, intIncrement),
      FloatRange range when y is Float floatIncrement => new FloatRange(range, floatIncrement.Value),
      UserObject userObject when y is UserObject stop => new KRange(new UserRangeItem(userObject), new UserCompare(stop), inclusive, increment),
      _ => incompatibleClasses(x, "RangeItem")
   };

   public override string ToString() => (StringStream)"new.range(" / inclusive.Extend("inclusive", "exclusive") / ")";
}