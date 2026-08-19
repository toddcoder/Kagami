using Core.Arrays;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Packages;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public class KEvent : IObject, IInvokableObject, ICollection
{
   protected Core.Applications.Messaging.MessageEvent<IObject> messageEvent = new();
   protected Maybe<Lambda> _handler = nil;
   protected List<Lambda> lambdas = [];

   public string ClassName => "Event";

   public string AsString => "event";

   public string Image => "event";

   public int Hash => messageEvent.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is KEvent otherEvent && messageEvent.Equals(otherEvent.messageEvent);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => false;

   public bool IsTrue => false;

   public Guid Id { get; init; } = Guid.NewGuid();

   public IObject GetHandler() => someOf(_handler.Map(IObject (l) => l));

   public Lambda Handler
   {
      set
      {
         messageEvent.Handler = o => value.Invoke(o);
         _handler = value;
      }
   }

   public KEvent SetHandler(Lambda lambda)
   {
      Handler = lambda;
      return this;
   }

   public IObject Add(Lambda lambda)
   {
      messageEvent.Add(o => lambda.Invoke(o));
      lambdas.Add(lambda);
      return this;
   }

   public IObject Remove(Lambda lambda)
   {
      messageEvent.Remove(o => lambda.Invoke(o));
      lambdas.Remove(lambda);
      return this;
   }

   public IObject Invoke(IObject argument)
   {
      messageEvent.Invoke(argument);
      return this;
   }

   public IInvokable Invokable
   {
      get
      {
         if (_handler is (true, var handler))
         {
            return handler.Invokable;
         }
         else
         {
            return Sys.IdLambda.Invokable;
         }
      }
   }

   public IObject this[SkipTake skipTake] => KArray.CreateObject(lambdas.Skip(skipTake.Skip).Take(skipTake.Take).Select(IObject (l) => l), nil);

   public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

   public Maybe<IObject> Next(int index) => lambdas[index];

   public Maybe<IObject> Peek(int index) => Next(index);

   public Int Length => lambdas.Count;

   public bool ExpandForArray => false;

   public KBoolean In(IObject item) => item is Lambda lambda && lambdas.Contains(lambda);

   public KBoolean NotIn(IObject item) => item is not Lambda lambda || !lambdas.Contains(lambda);

   public IObject Times(int count) => KArray.CreateObject(((Lambda[])[.. lambdas]).Repeat(count), nil);

   public KString MakeString(string connector) => lambdas.Select(l => l.AsString).ToString(connector);

   public IIterator GetIndexedIterator() => new IndexedIterator(this);

   public IObject One() => lambdas.Count == 1 ? lambdas[0] : this;

   public IObject Copy()
   {
      var kEvent = new KEvent();
      foreach (var lambda in lambdas)
      {
         kEvent.Add(lambda);
      }

      return kEvent;
   }

   public IIterator Following(IObject following) => new MultiIterator(this, following);

   public Maybe<TypeConstraint> TypeConstraint => nil;
}