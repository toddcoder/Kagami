using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public class UnmatchedClass : BaseClass
{
   public override string Name => "Unmatched";

   public override IObject DefaultValue => Unmatched.Value;
}