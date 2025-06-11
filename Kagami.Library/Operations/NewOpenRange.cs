using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
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
               _ => new SequenceIterator(i.Value, y)
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