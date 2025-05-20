using Kagami.Library.Invokables;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Monads;
using Core.Monads.Lazy;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class Lazy : IObject
{
   protected IInvokable invokable;
   protected Maybe<IObject> _value;

   public Lazy(IInvokable invokable)
   {
      this.invokable = invokable;
      _value = nil;
   }

   protected IObject getValue()
   {
      LazyOptional<IObject> _result = nil;
      if (_value is (true, var value))
      {
         return value;
      }
      else if (_result.ValueOf(Machine.Current.Value.Invoke(invokable, Arguments.Empty, 0)) is (true, var result))
      {
         _value = result.Some();
         return result;
      }
      else if (_result.Exception is (true, var exception))
      {
         throw exception;
      }
      else
      {
         throw fail("Value could not be resolved");
      }
   }

   public IObject Value => getValue();

   public string ClassName => getValue().ClassName;

   public string AsString => getValue().AsString;

   public string Image => getValue().Image;

   public int Hash => getValue().Hash;

   public bool IsEqualTo(IObject obj) => getValue().IsEqualTo(obj);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings)
   {
      if (comparisand is Placeholder placeholder)
      {
         bindings[placeholder.Name] = this;
         return true;
      }
      else
      {
         return getValue().Match(comparisand, bindings);
      }
   }

   public bool IsTrue => getValue().IsTrue;
}