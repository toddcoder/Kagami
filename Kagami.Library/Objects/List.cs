using System;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;

namespace Kagami.Library.Objects;

public class List : IObject, ICollection
{
   public static List Empty => new(nil, null);

   public static List Single(IObject value) => new(value.Some(), Empty);

   public static List Cons(IObject head, IObject tail)
   {
      if (tail is List list)
      {
         return new List(head.Some(), list);
      }
      else
      {
         return new List(head, tail);
      }
   }

   public static List NewList(IEnumerable<IObject> list)
   {
      var current = Empty;
      foreach (var value in list.Reverse())
      {
         current = Cons(value, current);
      }

      return current;
   }

   public static List NewList(Container list) => NewList(list.List);

   protected Maybe<IObject> _head;
   protected List tail;

   public List(Maybe<IObject> head, List tail)
   {
      _head = head;
      this.tail = tail;
   }

   public List(IObject head, IObject tail)
   {
      _head = head.Some();
      this.tail = Single(tail);
   }

   public bool IsString { get; set; }

   public Maybe<IObject> Head => _head;

   public List Tail => tail ?? Empty;

   public List Init
   {
      get
      {
         if (_head is (true, var head))
         {
            return tail.IsEmpty ? Empty : Cons(head, tail.Init);
         }
         else
         {
            return Empty;
         }
      }
   }

   public Maybe<IObject> Last
   {
      get
      {
         if (_head)
         {
            return tail.IsEmpty ? _head : tail.Last;
         }
         else
         {
            return nil;
         }
      }
   }

   public bool IsEmpty => !_head;

   public string ClassName => "List";

   protected string getText(string divider, Func<IObject, string> mapping, bool first = true)
   {
      if (_head is (true, var head))
      {
         return (first ? "" : divider) + $"{mapping(head)}{tail.getText(divider, mapping, false)}";
      }
      else
      {
         return "";
      }
   }

   public string AsString => IsString ? getText("", v => v.AsString) : getText(" ", v => v.AsString);

   public string Image => IsString ? show(this, "$\"", o => o.AsString, "\"") : show(this, "⌈", o => o.Image, "⌉");

   public int Hash => (_head.Map(h => h.Hash) | 0 + tail.Hash).GetHashCode();

   public bool IsEqualTo(IObject obj) => obj switch
   {
      List list when _head is (true, var h1) && list._head is (true, var h2) => h1.IsEqualTo(h2) && tail.IsEqualTo(list.tail),
      List list when !_head && !list._head => true,
      List => false,
      _ => false
   };

   protected static Maybe<string> getPlaceholder(IObject obj)
   {
      return obj is Placeholder placeholder ? placeholder.AsString : nil;
   }

   public bool Match(IObject comparisand, Hash<string, IObject> bindings)
   {
      return match(this, comparisand, (l1, l2) =>
      {
         if (!l1._head && !l2._head)
         {
            return true;
         }
         else
         {
            var lHead = l1._head.Map(v => v) | (() => Empty);
            var rHead = l2._head.Map(v => v) | (() => Empty);
            if (getPlaceholder(rHead) is (true, var placeholder))
            {
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
            }
            else if (rHead is Any || lHead.Match(rHead, bindings))
            {
               return l2.Tail.IsEmpty || l1.Tail.Match(l2.Tail, bindings);
            }
            else
            {
               return false;
            }
         }
      }, bindings);
   }

   public bool IsTrue => _head;

   public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new ListIterator(this);

   public Maybe<IObject> Next(int index) => nil;

   public Maybe<IObject> Peek(int index) => Next(index);

   public Int Length => list(this).Count();

   public bool ExpandForArray => true;

   public Boolean In(IObject item) => list(this).Any(i => i.IsEqualTo(item));

   public Boolean NotIn(IObject item) => list(this).All(i => !i.IsEqualTo(item));

   public IObject Times(int count)
   {
      var accum = Empty;
      for (var i = 0; i < count; i++)
      {
         accum = (List)accum.Concatenate(this);
      }

      return accum;
   }

   public String MakeString(string connector) => makeString(this, connector);

   public IIterator GetIndexedIterator() => new IndexedIterator(this);

   public IObject Concatenate(List other)
   {
      var left = GetIterator(false).List().ToList();
      left.AddRange(other.GetIterator(false).List());

      return NewList(left);
   }

   protected static IObject getItem(List list, int currentIndex, int expectedIndex)
   {
      if (list.IsEmpty)
      {
         return Unassigned.Value;
      }
      else
      {
         if (currentIndex < expectedIndex)
         {
            return getItem(list.Tail, currentIndex + 1, expectedIndex);
         }
         else if (list.Head is (true, var head))
         {
            return head;
         }
         else
         {
            return Unassigned.Value;
         }
      }
   }

   public IObject this[int index]
   {
      get
      {
         var item = (List)GetIterator(false).Skip(index);
         return someOf(item._head);
      }
   }

   public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);
}