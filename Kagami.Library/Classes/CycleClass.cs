using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class CycleClass : BaseClass, ICollectionClass
{
   public override string Name => "Cycle";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      collectionMessages();

      messages["items".get()] = (obj, _) => function<Cycle>(obj, c => c.Items);
   }

   public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Collection");

   public IObject Revert(IEnumerable<IObject> list) => new KTuple(list.ToArray());
}