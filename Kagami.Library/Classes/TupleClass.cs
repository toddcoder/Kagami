﻿using System.Collections.Generic;
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
         messages["tuple"] = (obj, _) => function<Tuple>(obj, t => t);
         messages["first".get()] = (obj, _) => function<Tuple>(obj, t => t[0]);
         messages["second".get()] = (obj, _) => function<Tuple>(obj, t => t[1]);
         messages["third".get()] = (obj, _) => function<Tuple>(obj, t => t[2]);
         messages["fourth".get()] = (obj, _) => function<Tuple>(obj, t => t[3]);
         messages["fifth".get()] = (obj, _) => function<Tuple>(obj, t => t[4]);
         messages["sixth".get()] = (obj, _) => function<Tuple>(obj, t => t[5]);
         messages["seventh".get()] = (obj, _) => function<Tuple>(obj, t => t[6]);
         messages["eighth".get()] = (obj, _) => function<Tuple>(obj, t => t[7]);
         messages["ninth".get()] = (obj, _) => function<Tuple>(obj, t => t[8]);
         messages["tenth".get()] = (obj, _) => function<Tuple>(obj, t => t[9]);
         messages["[]"] = (obj, msg) => function<Tuple, IObject>(obj, msg, indexed);
         messages["head".get()] = (obj, _) => function<Tuple>(obj, t => t.Head);
         messages["tail".get()] = (obj, _) => function<Tuple>(obj, t => t.Tail);
         messages["headTail".get()] = (obj, _) => function<Tuple>(obj, t => t.HeadTail);
      }

      protected static IObject indexed(Tuple tuple, IObject index) => index switch
      {
         Int i => tuple[i.Value],
         String s => tuple[s.Value],
         _ => throw "Invalid index".Throws()
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