﻿using Kagami.Library.Objects;
using Standard.Types.Exceptions;
using static Kagami.Library.Classes.ClassFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Classes
{
   public class TupleClass : BaseClass
   {
      public override string Name => "Tuple";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         collectionMessages();

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
               return someOf(tuple[s.Value]);
            default:
               throw "Invalid index".Throws();
         }
      }

      public override bool DynamicRespondsTo(string message) => true;

      public override IObject DynamicInvoke(IObject obj, Message message)
      {
         if (base.DynamicRespondsTo(message.Name))
            return base.DynamicInvoke(obj, message);
         else
         {
            var tuple = (Tuple)obj;
            var name = message.Name.unget();
            return tuple[name].FlatMap(v => new Some(v), () => Nil.NilValue);
         }
      }
   }
}