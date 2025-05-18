using Kagami.Library.Objects;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class NewDataComparisand : MultipleOperandOperation
{
   protected string className;
   protected string name;
   protected IObject[] comparisands;
   protected IObject ordinal;

   public NewDataComparisand() : base(4)
   {
   }

   public override Result<Unit> Execute(int index, IObject value) => index switch
   {
      0 => match<String>(value, s => className = s.Value),
      1 => match<String>(value, s => name = s.Value),
      2 => match<Arguments>(value, t => comparisands = t.Value),
      3 => match<IObject>(value, o => ordinal = o),
      _ => unit
   };

   public override Optional<IObject> Final()
   {
      return new DataComparisand(className, name, comparisands, ordinal);
   }

   public override string ToString() => "new.data.comparisand";
}