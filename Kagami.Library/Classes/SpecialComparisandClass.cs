using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Classes;

public class SpecialComparisandClass : BaseClass
{
   public override string Name => "SpecialComparisand";

   public override IObject DefaultValue => throw noDefaultValue(Name);
}