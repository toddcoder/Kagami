using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class NewOpenRange : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      switch (x)
      {
         case Int i:
            return y switch
            {
               Lambda lambda1 => new OpenRange(x, lambda1),
               INumeric numeric => new NumericOpenRange(i, numeric),
               Undefined => fail("new.open.range Int failed"),
               _ => new SequenceIterator(i.Value, y)
            };
         case INumeric seed:
            return y switch
            {
               INumeric incrementer => new NumericOpenRange(seed, incrementer),
               Lambda lambda1 => new OpenRange(Int.IntObject(seed.AsInt32()), lambda1),
               Undefined => fail("new.open.range Numeric failed"),
               _ => new SequenceIterator(seed.AsInt32(), y)
            };
         case SequenceIterator seq:
            return new SequenceIterator(seq, y);
         default:
            if (y is Lambda lambda2)
            {
               return new OpenRange(x, lambda2);
            }
            else
            {
               return incompatibleClasses(y, "Lambda");
            }
      }
   }

   public override string ToString() => "new.open.range";
}