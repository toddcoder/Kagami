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
      messages["in(_)"] = (obj, msg) => function<KRange, IObject>(obj, msg, (r, o) => r.In(o));
      messages["notIn(_)"] = (obj, msg) => function<KRange, IObject>(obj, msg, (r, o) => r.NotIn(o));
      messages["+(_)"] = (obj, msg) => function<KRange, Int>(obj, msg, (r, i) => r.Add(i.Value));
      messages["-(_)"] = (obj, msg) => function<KRange, Int>(obj, msg, (r, i) => r.Subtract(i.Value));
      messages["inverse()"] = (obj, _) => function<KRange>(obj, r => r.Reverse());
      messages["~(_)"] = (obj, msg) => function<KRange, KRange>(obj, msg, (r1, r2) => r1.Concatenate(r2));
      messages["max".get()] = (obj, _) => function<KRange>(obj, r => r.Max());
   }

   public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Collection");

   public IObject Revert(IEnumerable<IObject> list) => new KArray(list.ToList());
}