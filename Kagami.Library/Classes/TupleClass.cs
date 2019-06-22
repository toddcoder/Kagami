using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Objects;
using Core.Exceptions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class TupleClass : BaseClass, ICollectionClass
   {
      public override string Name => "Tuple";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         collectionMessages();
         messages["tuple"] = (obj, msg) => function<Tuple>(obj, t => t);

         messages["first".get()] = (obj, msg) => function<Tuple>(obj, t => t[0]);
         messages["second".get()] = (obj, msg) => function<Tuple>(obj, t => t[1]);
         messages["third".get()] = (obj, msg) => function<Tuple>(obj, t => t[2]);
         messages["fourth".get()] = (obj, msg) => function<Tuple>(obj, t => t[3]);
         messages["fifth".get()] = (obj, msg) => function<Tuple>(obj, t => t[4]);
         messages["sixth".get()] = (obj, msg) => function<Tuple>(obj, t => t[5]);
         messages["seventh".get()] = (obj, msg) => function<Tuple>(obj, t => t[6]);
         messages["eighth".get()] = (obj, msg) => function<Tuple>(obj, t => t[7]);
         messages["nineth".get()] = (obj, msg) => function<Tuple>(obj, t => t[8]);
         messages["tenth".get()] = (obj, msg) => function<Tuple>(obj, t => t[9]);
         messages["[]"] = (obj, msg) => function<Tuple, IObject>(obj, msg, indexed);
      }

      static IObject indexed(Tuple tuple, IObject index)
      {
         switch (index)
         {
            case Int i:
               return tuple[i.Value];
            case String s:
               return tuple[s.Value];
            default:
               throw "Invalid index".Throws();
         }
      }

      public override bool DynamicRespondsTo(Selector selector) => true;

      public override IObject DynamicInvoke(IObject obj, Message message)
      {
         if (base.DynamicRespondsTo(message.Selector))
         {
	         return base.DynamicInvoke(obj, message);
         }
         else
         {
            var tuple = (Tuple)obj;
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

      public IObject Revert(IEnumerable<IObject> list) => new Tuple(list.ToArray());

      public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Collection");
   }
}