﻿using System.Collections.Generic;
using Kagami.Library.Classes;
using Core.Collections;
using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;

namespace Kagami.Library.Objects
{
   public class ObjectCollection : IObject, ICollection
   {
      protected UserObject obj;
      protected BaseClass cls;

      public ObjectCollection(UserObject obj)
      {
         this.obj = obj;
         cls = classOf(this.obj);
      }

      public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

      public IMaybe<IObject> Next(int index)
      {
         var result = sendMessage(obj, "next", (Int)index);
         if (result is Some some)
         {
            return some.Value.Some();
         }
         else
         {
            return none<IObject>();
         }
      }

      public IMaybe<IObject> Peek(int index)
      {
         if (cls.RespondsTo("peek"))
         {
            var result = sendMessage(obj, "next", (Int)index);
            if (result is Some some)
            {
               return some.Value.Some();
            }
            else
            {
               return none<IObject>();
            }
         }
         else
         {
            return none<IObject>();
         }
      }

      public Int Length => cls.RespondsTo("length".get()) ? (Int)sendMessage(obj, "length".get()) : -1;

      public IEnumerable<IObject> List
      {
         get
         {
            var i = 0;
            while (true)
            {
               if (Next(i++).If(out var value))
               {
                  yield return value;
               }
               else
               {
                  yield break;
               }
            }
         }
      }

      public bool ExpandForArray => true;

      public Boolean In(IObject item) => cls.RespondsTo("in") ? (Boolean)sendMessage(obj, "in", item) : false;

      public Boolean NotIn(IObject item) => cls.RespondsTo("notIn") ? (Boolean)sendMessage(obj, "notIn", item) : false;

      public IObject Times(int count) => cls.RespondsTo("*") ? (Boolean)sendMessage(obj, "*", (Int)count) : obj;

      public String MakeString(string connector) => makeString(this, connector);

      public IIterator GetIndexedIterator() => new IndexedIterator(this);

      public string ClassName => obj.ClassName;

      public string AsString => obj.AsString;

      public string Image => obj.Image;

      public int Hash => obj.Hash;

      public bool IsEqualTo(IObject obj) => this.obj.IsEqualTo(obj);

      public bool Match(IObject comparisand, Hash<string, IObject> bindings)
      {
         return match(this, comparisand, (oc1, oc2) => oc1.IsEqualTo(oc2), bindings);
      }

      public bool IsTrue => obj.IsTrue;

      public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);
   }
}