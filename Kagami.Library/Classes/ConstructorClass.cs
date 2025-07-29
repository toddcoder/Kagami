using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Classes;

public class ConstructorClass : BaseClass
{
   public override string Name => "Constructor";

   public override IObject DefaultValue => throw noDefaultValue("Constructor");
}