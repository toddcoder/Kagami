using System.Collections.Generic;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;

namespace Kagami.Library.Invokables;

public class YieldingInvokable : IInvokable, ICollection, IObject
{
   protected Selector selector;
   protected List<IObject> cached = [];

   public YieldingInvokable(Selector selector, Parameters parameters, string image)
   {
      this.selector = selector;
      Parameters = parameters;
      Image = image;
   }

   public Selector Selector => selector;

   public int Index { get; set; } = -1;

   public int Address { get; set; } = -1;

   public Parameters Parameters { get; }

   public string ClassName => "YieldingInvokable";

   public string AsString => selector.AsString;

   public string Image { get; }

   public bool Constructing => false;

   public int Hash => selector.Hash;

   public bool IsEqualTo(IObject obj) => obj is YieldingInvokable yfi && selector.IsEqualTo(yfi.selector);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => false;

   public bool IsTrue => false;

   public Arguments Arguments { get; set; } = Arguments.Empty;

   public FrameGroup Frames { get; set; } = new();

   public IIterator GetIterator(bool lazy)
   {
      var clone = new YieldingInvokable(selector, Parameters, Image) { Address = Address, Arguments = Arguments, Index = Index };
      return lazy ? new LazyIterator(clone) : new Iterator(clone);
   }

   public Maybe<IObject> Next(int index)
   {
      var _result = Machine.Current.Invoke(this);
      if (_result is (true, var result))
      {
         switch (result)
         {
            case None:
               return nil;
            case YieldReturn yr:
               Address = yr.Address + 1;
               Frames = yr.Frames;
               return yr.ReturnValue.Some();
            default:
               throw incompatibleClasses(result, "YieldReturn");
         }
      }
      else if (_result.Exception is (true, var exception))
      {
         throw exception;
      }
      else
      {
         return nil;
      }
   }

   public Maybe<IObject> Peek(int index) => throw fail("Peek not supported");

   public Int Length => cached.Count;

   public IEnumerable<IObject> List
   {
      get
      {
         var iterator = GetIterator(false);
         cached.Clear();

         while (true)
         {
            var _next = iterator.Next();
            if (_next is (true, var next))
            {
               cached.Add(next);
               yield return next;
            }
            else
            {
               yield break;
            }
         }
      }
   }

   public bool ExpandForArray => true;

   public Boolean In(IObject item) => cached.Contains(item);

   public Boolean NotIn(IObject item) => !cached.Contains(item);

   public IObject Times(int count) => this;

   public String MakeString(string connector) => makeString(this, connector);

   public IIterator GetIndexedIterator() => new IndexedIterator(this);

   public IObject this[SkipTake skipTake] => Objects.CollectionFunctions.skipTake(this, skipTake);
}