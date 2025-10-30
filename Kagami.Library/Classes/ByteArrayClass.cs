using Core.Monads;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class ByteArrayClass : BaseClass, ICollectionClass
{
   public override string Name => "ByteArray";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      collectionMessages();
      compareMessages();

      messages["[](_)"] = (obj, msg) => function<ByteArray, Int>(obj, msg, (b, i) => b[i.Value]);
      messages["~(_<ByteArray>)"] = (obj, msg) => function<ByteArray, ByteArray>(obj, msg, (b1, b2) => b1.Concatenate(b2));
      messages["encode(_)"] = (obj, msg) => function<ByteArray, KString>(obj, msg, (b, e) => b.Encode(e.Value));
   }

   public override IObject DefaultValue => new ByteArray([]);

   public IObject Revert(IEnumerable<IObject> list, Maybe<TypeConstraint> _typeConstraint) => new ByteArray([.. list.Select(o => (KByte)o).Select(b => b.Value)]);

   public TypeConstraint EquivalentTypeConstraint() => TypeConstraint.FromList("Collection");
}