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
      IRangeItem start when y is Any => new KRange(start, new Infinity(true), inclusive),
      IRangeItem start when y is IObjectCompare stop => new KRange(start, stop, inclusive),
      KRange range when y is Int increment => new KRange(range, increment),
      UserObject userObject when y is UserObject stop => new KRange(new UserRangeItem(userObject), new UserCompare(stop), inclusive),
      _ => incompatibleClasses(x, "RangeItem")
   };

   public override string ToString() => (StringStream)"new.range(" / inclusive.Extend("inclusive", "exclusive") / ")";
}