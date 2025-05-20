using Core.Objects;
using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public class ByteClass : BaseClass, IParse, IEquivalentClass
{
   public override string Name => "Byte";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      compareMessages();
   }

   public override void RegisterClassMessages()
   {
      base.RegisterClassMessages();

      classMessages["min".get()] = (_, _) => (KByte)byte.MinValue;
      classMessages["max".get()] = (_, _) => (KByte)byte.MaxValue;
   }

   public IObject Parse(string source) => KByte.ByteObject(source.Value().Byte());

   public override bool IsNumeric => true;

   public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Number");
}