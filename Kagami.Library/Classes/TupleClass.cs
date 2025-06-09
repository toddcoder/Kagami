using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class TupleClass : BaseClass, ICollectionClass
{
   public override string Name => "Tuple";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      collectionMessages();
      messages["tuple"] = (obj, _) => function<KTuple>(obj, t => t);
      messages["first".get()] = (obj, _) => function<KTuple>(obj, t => t[0]);
      messages["second".get()] = (obj, _) => function<KTuple>(obj, t => t[1]);
      messages["third".get()] = (obj, _) => function<KTuple>(obj, t => t[2]);
      messages["fourth".get()] = (obj, _) => function<KTuple>(obj, t => t[3]);
      messages["fifth".get()] = (obj, _) => function<KTuple>(obj, t => t[4]);
      messages["sixth".get()] = (obj, _) => function<KTuple>(obj, t => t[5]);
      messages["seventh".get()] = (obj, _) => function<KTuple>(obj, t => t[6]);
      messages["eighth".get()] = (obj, _) => function<KTuple>(obj, t => t[7]);
      messages["ninth".get()] = (obj, _) => function<KTuple>(obj, t => t[8]);
      messages["tenth".get()] = (obj, _) => function<KTuple>(obj, t => t[9]);
      messages["[](_)"] = (obj, msg) => function<KTuple, IObject>(obj, msg, indexed);
      messages["head".get()] = (obj, _) => function<KTuple>(obj, t => t.Head);
      messages["tail".get()] = (obj, _) => function<KTuple>(obj, t => t.Tail);
      messages["headTail".get()] = (obj, _) => function<KTuple>(obj, t => t.HeadTail);
   }

   protected static IObject indexed(KTuple kTuple, IObject index) => index switch
   {
      Int i => kTuple[i.Value],
      KString s => kTuple[s.Value],
      _ => throw fail("Invalid index")
   };

   public override bool DynamicRespondsTo(Selector selector) => true;

   public override IObject DynamicInvoke(IObject obj, Message message)
   {
      if (base.DynamicRespondsTo(message.Selector))
      {
         return base.DynamicInvoke(obj, message);
      }
      else
      {
         var tuple = (KTuple)obj;
         var name = message.Selector.Name.unget();
         if (tuple.ContainsName(name))
         {
            return tuple[name];
         }
         else
         {
            throw messageNotFound(this, name);
         }
      }
   }

   public IObject Revert(IEnumerable<IObject> list) => new KTuple(list.ToArray());

   public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Collection");
}