using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Classes;

public class SelectorClass : BaseClass
{
   public override string Name => "Selector";

   public override IObject DefaultValue => throw noDefaultValue("Selector");
}