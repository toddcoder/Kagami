using Kagami.Library.Classes;
using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Objects.CollectionFunctions;

namespace Kagami.Library.Objects;

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

   public Maybe<IObject> Next(int index)
   {
      var result = sendMessage(obj, "next(_)", (Int)index);
      if (result is Some some)
      {
         return some.Value.Some();
      }
      else
      {
         return nil;
      }
   }

   public Maybe<IObject> Peek(int index)
   {
      if (cls.RespondsTo("peek(_)"))
      {
         var result = sendMessage(obj, "peek(_)", (Int)index);
         if (result is Some some)
         {
            return some.Value.Some();
         }
         else
         {
            return nil;
         }
      }
      else
      {
         return nil;
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
            if (Next(i++) is (true, var value))
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

   public KBoolean In(IObject item) => cls.RespondsTo("in(_)") ? (KBoolean)sendMessage(obj, "in(_)", item) : false;

   public KBoolean NotIn(IObject item) => cls.RespondsTo("notIn(_)") ? (KBoolean)sendMessage(obj, "notIn(_)", item) : false;

   public IObject Times(int count) => cls.RespondsTo("*(_)") ? (KBoolean)sendMessage(obj, "*(_)", (Int)count) : obj;

   public KString MakeString(string connector) => makeString(this, connector);

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

   public Guid Id { get; init; } = Guid.NewGuid();

   public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);
}