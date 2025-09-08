using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public class ConsClass : BaseClass
{
   public override string Name => "Cons";

   public override IObject DefaultValue => new Cons(KNil.NilValue, KArray.Empty);
}