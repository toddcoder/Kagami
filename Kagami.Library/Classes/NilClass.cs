﻿using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class NilClass : BaseClass
   {
      public override string Name => "Nil";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         messages["isSome".get()] = (obj, msg) => function<Nil>(obj, n => (Boolean)n.IsSome);
         messages["isNil".get()] = (obj, msg) => function<Nil>(obj, n => (Boolean)n.IsNil);
         messages["map"] = (obj, msg) => function<Nil, Lambda>(obj, msg, (n, l) => n.Map(l));
         messages["flatMap"] = (obj, msg) => function<Nil, Lambda, Lambda>(obj, msg, (n, l1, l2) => n.FlatMap(l1, l2));
      }

      public override bool AssignCompatible(BaseClass otherClass) => otherClass is SomeClass || otherClass is NilClass;
   }
}