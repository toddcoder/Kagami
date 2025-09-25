using Core.Objects;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class ByteClass : BaseClass, IParse, IEquivalentClass
{
   public override string Name => "Byte";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      compareMessages();

      messages["char()"] = (obj, _) => function<KByte>(obj, b => new KChar((char)b.Value));
      messages["numberize()"] = (obj, _) => function<KByte>(obj, b => new KChar((char)b.Value));
   }

   public override void RegisterClassMessages()
   {
      base.RegisterClassMessages();

      classMessages["min".get()] = (_, _) => (KByte)byte.MinValue;
      classMessages["max".get()] = (_, _) => (KByte)byte.MaxValue;
   }

   public IObject Parse(string source) => KByte.ByteObject(source.Value().Byte());

   public override bool IsNumeric => true;

   public override IObject DefaultValue => new KByte(0);

   public TypeConstraint EquivalentTypeConstraint() => TypeConstraint.FromList("Number");
}