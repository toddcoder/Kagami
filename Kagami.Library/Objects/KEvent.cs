using Core.Collections;
using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Packages;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public class KEvent : IObject, IInvokableObject
{
   protected Core.Applications.Messaging.MessageEvent<IObject> messageEvent = new();
   protected Maybe<Lambda> _handler = nil;

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

   public IObject Add(Lambda lambda)
   {
      messageEvent.Add(o => lambda.Invoke(o));
      return this;
   }

   public IObject Remove(Lambda lambda)
   {
      messageEvent.Remove(o => lambda.Invoke(o));
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
}