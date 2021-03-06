﻿using System.Collections.Generic;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects
{
   public class ListIterator : Iterator
   {
      protected List list;

      public ListIterator(List list) : base(list) => this.list = list;

      public override IMaybe<IObject> Next()
      {
         if (list.Head.If(out var head))
         {
            list = list.Tail;
            return head.Some();
         }
         else
         {
            return none<IObject>();
         }
      }

      public override IMaybe<IObject> Peek() => list.Head;

      public override IEnumerable<IObject> List()
      {
         var current = list;
         while (current.Head.If(out var head))
         {
            yield return head;

            current = current.Tail;
         }
      }
   }
}