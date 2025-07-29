using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Classes;

public class ForwardedClass : BaseClass
{
   public ForwardedClass(string name) => Name = name;

   public override string Name { get; }

   public override IObject DefaultValue => throw noDefaultValue("name");
}