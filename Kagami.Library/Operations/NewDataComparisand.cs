using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations
{
   public class NewDataComparisand : MultipleOperandOperation
   {
      string className;
      string name;
      IObject[] comparisands;
      IObject ordinal;

      public NewDataComparisand() : base(4) { }

      public override IResult<Unit> Execute(Machine machine, int index, IObject value)
      {
         switch (index)
         {
            case 0:
               return match<String>(value, s => className = s.Value);
            case 1:
               return match<String>(value, s => name = s.Value);
            case 2:
               return match<Arguments>(value, t => comparisands = t.Value);
            case 3:
               return match<IObject>(value, o => ordinal = o);
         }

         return Unit.Success();
      }

      public override IMatched<IObject> Final(Machine machine)
      {
         return new DataComparisand(className, name, comparisands, ordinal).Matched<IObject>();
      }

      public override string ToString() => "new.data.comparisand";
   }
}