﻿using System.Collections.Generic;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Classes
{
   public class ListClass : BaseClass, ICollectionClass
   {
      public override string Name => "List";

      public IObject Revert(IEnumerable<IObject> list) => List.NewList(list);

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         collectionMessages();

         messages["head".get()] = (obj, msg) => function<List>(obj, l => someOf(l.Head));
         messages["tail".get()] = (obj, msg) => function<List>(obj, l => l.Tail);
         messages["~"] = (obj, msg) => function<List, List>(obj, msg, (l1, l2) => l1.Concatenate(l2));
      }
   }
}