using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Classes;

public class YieldReturnClass : BaseClass
{
   public override string Name => "YieldReturn";

   public override IObject DefaultValue => throw noDefaultValue("YieldReturn");
}