using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Classes;

public class DefinitionClass : BaseClass
{
   public override string Name => "Definition";

   public override IObject DefaultValue => throw noDefaultValue(Name);
}