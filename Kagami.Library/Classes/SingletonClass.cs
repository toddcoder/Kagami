using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Classes;

public class SingletonClass : BaseClass
{
   public override string Name => "Singleton";

   public override IObject DefaultValue => throw noDefaultValue(Name);
}