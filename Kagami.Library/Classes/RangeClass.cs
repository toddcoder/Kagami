using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class RangeClass : BaseClass, ICollectionClass
{
   public override string Name => "Range";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      collectionMessages();

      messages["start".get()] = (obj, _) => function<KRange>(obj, r => r.StartObj);
      messages["stop".get()] = (obj, _) => function<KRange>(obj, r => r.StopObj);
      messages["increment".get()] = (obj, _) => function<KRange>(obj, r => (Int)r.Increment);
      messages["in"] = (obj, msg) => function<KRange, IObject>(obj, msg, (r, o) => r.In(o));
      messages["notIn"] = (obj, msg) => function<KRange, IObject>(obj, msg, (r, o) => r.NotIn(o));
      messages["+"] = (obj, msg) => function<KRange, Int>(obj, msg, (r, i) => r.Add(i.Value));
      messages["-"] = (obj, msg) => function<KRange, Int>(obj, msg, (r, i) => r.Subtract(i.Value));
      messages["inverse()"] = (obj, _) => function<KRange>(obj, r => r.Reverse());
   }

   public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Collection");

   public IObject Revert(IEnumerable<IObject> list) => new KArray([.. list]);
}