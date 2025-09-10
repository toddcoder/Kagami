using System.Numerics;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class LongRangeClass : BaseClass, ICollectionClass
{
   public override string Name => "LongRange";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      collectionMessages();

      messages["start".get()] = (obj, _) => function<LongRange>(obj, r => r.StartObj);
      messages["stop".get()] = (obj, _) => function<LongRange>(obj, r => r.StopObj);
      messages["increment".get()] = (obj, _) => function<LongRange>(obj, r => (Long)r.Increment);
      messages["in(_)"] = (obj, msg) => function<LongRange, IObject>(obj, msg, (r, o) => r.In(o));
      messages["notIn(_)"] = (obj, msg) => function<LongRange, IObject>(obj, msg, (r, o) => r.NotIn(o));
      messages["+(_)"] = (obj, msg) => function<LongRange, Long>(obj, msg, (r, i) => r.Add(i.Value));
      messages["-(_)"] = (obj, msg) => function<LongRange, Long>(obj, msg, (r, i) => r.Subtract(i.Value));
      messages["inverse()"] = (obj, _) => function<LongRange>(obj, r => r.Reverse());
      messages["~(_)"] = (obj, msg) => function<LongRange, LongRange>(obj, msg, (r1, r2) => r1.Concatenate(r2));
      messages["max".get()] = (obj, _) => function<LongRange>(obj, r => r.Max());
   }

   public override IObject DefaultValue => new LongRange(BigInteger.Zero, BigInteger.Zero, true, BigInteger.One);

   public TypeConstraint EquivalentTypeConstraint() => TypeConstraint.FromList("Collection");

   public IObject Revert(IEnumerable<IObject> list) => new KArray(list.ToList());
}