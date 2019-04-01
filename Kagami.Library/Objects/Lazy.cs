using Kagami.Library.Invokables;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Exceptions;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects
{
   public class Lazy : IObject
   {
      IInvokable invokable;
      IMaybe<IObject> value;

      public Lazy(IInvokable invokable)
      {
         this.invokable = invokable;
         value = none<IObject>();
      }

      IObject getValue()
      {
	      if (value.If(out var v))
		      return v;
	      else if (Machine.Current.Invoke(invokable, Arguments.Empty, 0).If(out var result, out var mbException))
	      {
		      value = result.Some();
		      return result;
	      }
	      else if (mbException.If(out var exception))
		      throw exception;
	      else
		      throw "Value could not be resolved".Throws();
      }

      public IObject Value => getValue();

      public string ClassName => getValue().ClassName;

      public string AsString => getValue().AsString;

      public string Image => getValue().Image;

      public int Hash => getValue().Hash;

      public bool IsEqualTo(IObject obj) => getValue().IsEqualTo(obj);

      public bool Match(IObject comparisand, Hash<string, IObject> bindings)
      {
         if (comparisand is Placeholder ph)
         {
            bindings[ph.Name] = this;
            return true;
         }
         else
            return getValue().Match(comparisand, bindings);
      }

      public bool IsTrue => getValue().IsTrue;
   }
}