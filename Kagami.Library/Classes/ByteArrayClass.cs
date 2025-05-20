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

      messages["[]"] = (obj, msg) => function<ByteArray, Int>(obj, msg, (b, i) => b[i.Value]);
   }

   public IObject Revert(IEnumerable<IObject> list) => new ByteArray(list.Select(o => (KByte)o).Select(b => b.Value).ToArray());

   public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Collection");
}