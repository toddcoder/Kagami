using System;
using System.Collections.Generic;
using System.Linq;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Objects
{
   public class List : IObject, ICollection
   {
      public static List Empty => new List(none<IObject>(), null);

      public static List Single(IObject value) => new List(value.Some(), Empty);

      public static List Cons(IObject head, IObject tail)
      {
         if (tail is List list)
            return new List(head.Some(), list);
         else
            return new List(head, tail);
      }

      public static List NewList(IEnumerable<IObject> list)
      {
         var current = Empty;
         foreach (var value in list.Reverse())
            current = Cons(value, current);

         return current;
      }

      public static List NewList(InternalList list) => NewList(list.List);

      IMaybe<IObject> head;
      List tail;

      public List(IMaybe<IObject> head, List tail)
      {
         this.head = head;
         this.tail = tail;
      }

      public List(IObject head, IObject tail)
      {
         this.head = head.Some();
         this.tail = Single(tail);
      }

      public bool IsString { get; set; }

      public IMaybe<IObject> Head => head;

      public List Tail => tail ?? Empty;

      public bool IsEmpty => head.IsNone;

      public string ClassName => "List";

      string getText(string divider, Func<IObject, string> mapping, bool first = true)
      {
         if (head.If(out var h))
            return (first ? "" : divider) + $"{mapping(h)}{tail.getText(divider, mapping, false)}";
         else
            return "";
      }

      public string AsString
      {
         get
         {
            if (IsString)
               return getText("", v => v.AsString);
            else
               return getText(" ", v => v.AsString);
         }
      }

      public string Image
      {
         get
         {
            if (IsString)
               return show(this, "$\"", o => o.AsString, "\"");
            else
               return show(this, "[", o => o.Image, "]");
         }
      }

      public int Hash => (head.FlatMap(h => h.Hash, () => 0) + tail.Hash).GetHashCode();

      public bool IsEqualTo(IObject obj)
      {
         switch (obj)
         {
            case List list when head.If(out var h1) && list.head.If(out var h2):
               return h1.IsEqualTo(h2) && tail.IsEqualTo(list.tail);
            case List list when head.IsNone && list.head.IsNone:
               return true;
            case List _:
               return false;
            default:
               return false;
         }
      }

      static IMaybe<string> getPlaceholder(IObject obj)
      {
         return obj is Placeholder placeholder ? placeholder.AsString.Some() : none<string>();
      }

      public bool Match(IObject comparisand, Hash<string, IObject> bindings)
      {
         return match(this, comparisand, (l1, l2) =>
         {
            if (l1.head.IsNone && l2.head.IsNone)
               return true;
            else
            {
               var lHead = l1.head.FlatMap(v => v, () => Empty);
               var rHead = l2.head.FlatMap(v => v, () => Empty);
               if (getPlaceholder(rHead).If(out var placeholder))
                  if (l2.tail.IsEmpty)
                  {
                     bindings[placeholder] = l1;
                     return true;
                  }
                  else
                  {
                     bindings[placeholder] = lHead;
                     return l1.Tail.Match(l2.Tail, bindings);
                  }
               else if (rHead is Any || lHead.Match(rHead, bindings))
                  return l2.Tail.IsEmpty || l1.Tail.Match(l2.Tail, bindings);
               else
                  return false;
            }
         }, bindings);
      }

      public bool IsTrue => head.IsSome;

      public IIterator GetIterator(bool lazy) => lazy ? (IIterator)new LazyIterator(this) : new ListIterator(this);

      public IMaybe<IObject> Next(int index) => none<IObject>();

      public IMaybe<IObject> Peek(int index) => Next(index);

      public Int Length => list(this).Count();

      public bool ExpandForArray => true;

      public Boolean In(IObject item) => list(this).Any(i => i.IsEqualTo(item));

      public Boolean NotIn(IObject item) => list(this).All(i => !i.IsEqualTo(item));

      public IObject Times(int count) => this;

      public IObject Flatten() => flatten(this);

      public IObject Concatenate(List other)
      {
         var left = GetIterator(false).List().ToList();
         left.AddRange(other.GetIterator(false).List());

         return NewList(left);
      }

      static IObject getItem(List list, int currentIndex, int expectedIndex)
      {
         if (list.IsEmpty)
            return Unassigned.Value;
         else
         {
            if (currentIndex < expectedIndex)
               return getItem(list.Tail, currentIndex + 1, expectedIndex);
            else if (list.Head.If(out var head))
               return head;
            else
               return Unassigned.Value;
         }
      }

      public IObject this[int index] => getItem(this, 0, index);
   }
}