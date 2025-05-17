using System.Collections.Generic;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class ListIterator : Iterator
{
   protected List list;

   public ListIterator(List list) : base(list) => this.list = list;

   public override Maybe<IObject> Next()
   {
      if (list.Head is (true, var head))
      {
         list = list.Tail;
         return head.Some();
      }
      else
      {
         return nil;
      }
   }

   public override Maybe<IObject> Peek() => list.Head;

   public override IEnumerable<IObject> List()
   {
      var current = list;
      while (current.Head is (true, var head))
      {
         yield return head;

         current = current.Tail;
      }
   }
}