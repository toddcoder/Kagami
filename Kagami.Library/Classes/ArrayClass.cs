using Kagami.Library.Objects;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Classes.ClassFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Classes;

public class ArrayClass : BaseClass, ICollectionClass
{
   public override string Name => "Array";

   public IObject Revert(IEnumerable<IObject> list) => KArray.CreateObject(list);

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      collectionMessages();
      messages["array()"] = (obj, _) => function<KArray>(obj, a => a);
      mutableCollectionMessages();
      sliceableMessages();
      findAndIndexMessages();

      messages["[](_)"] = (obj, msg) => function<KArray, IObject>(obj, msg, getIndexed);
      messages["get(_)"] = (obj, msg) => function<KArray, IObject>(obj, msg, (a, i) => someOf(a.Get(i)));
      messages["[]=(_,_)"] = (obj, msg) => function<KArray, IObject, IObject>(obj, msg, setIndexed);
      messages["~(_)"] = (obj, msg) => function<KArray, KArray>(obj, msg, (a1, a2) => a1.Concatenate(a2));
      registerMessage("push(_)", (obj, msg) => function<KArray, IObject>(obj, msg, (a, v) => a.Append(v)));
      registerMessage("pop()", (obj, _) => function<KArray>(obj, a => a.Pop()));
      registerMessage("unshift(_)", (obj, msg) => function<KArray, IObject>(obj, msg, (a, v) => a.Unshift(v)));
      registerMessage("shift()", (obj, _) => function<KArray>(obj, a => a.Shift()));
      messages["default".get()] = (obj, _) => function<KArray>(obj, array =>
      {
         if (array.DefaultValue is (true, var defaultValue))
         {
            return defaultValue;
         }
         else if (array.DefaultLambda is (true, var defaultLambda))
         {
            return defaultLambda;
         }
         else
         {
            return Unassigned.Value;
         }
      });
      messages["default".set()] = (obj, msg) => function<KArray, IObject>(obj, msg, (array, v) =>
      {
         if (v is Lambda lambda)
         {
            array.DefaultLambda = lambda;
         }
         else
         {
            array.DefaultValue = v.Some();
         }

         return KVoid.Value;
      });
      messages["transpose()"] = (obj, _) => function<KArray>(obj, a => a.Transpose());
   }

   protected static IObject getIndexed(KArray a, IObject i)
   {
      return CollectionFunctions.getIndexed(a, i, (array, index) => array[index], (array, list) => array[list]);
   }

   protected static IObject setIndexed(KArray a, IObject i, IObject v)
   {
      CollectionFunctions.setIndexed(a, i, v, (array, index, value) => array[index] = value, (array, list, value) => array[list] = value);
      return a;
   }

   public override void RegisterClassMessages()
   {
      base.RegisterClassMessages();

      classMessages["repeat(value:_,times:_<Int>)"] = (bc, msg) =>
         classFunc<ArrayClass, IObject, Int>(bc, msg, (_, v, t) => KArray.Repeat(v, t.Value));
      classMessages["empty".get()] = (bc, _) => classFunc<ArrayClass>(bc, _ => KArray.Empty);
      classMessages["typed(_)"] = (_, msg) => getTypedArray(msg);
   }

   protected static KArray getTypedArray(Message message)
   {
      if (message.Arguments[0] is TypeConstraint typeConstraint)
      {
         return new KArray([]) { TypeConstraint = typeConstraint.Some() };
      }
      else
      {
         throw fail("Type constraint for array required");
      }
   }

   public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Collection");
}