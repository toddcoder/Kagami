using Kagami.Library.Invokables;
using Kagami.Library.Runtime;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

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
         else
         {
            var result = Machine.Current.Invoke(invokable, Arguments.Empty, 0).Value;
            value = result.Some();
            return result;
         }
      }

      public string ClassName => value.FlatMap(v => v.ClassName, () => "Lazy");

      public string AsString => getValue().AsString;

      public string Image => value.FlatMap(v => v.Image, () => "pending");

      public int Hash => value.FlatMap(v => v.Hash, () => "pending".GetHashCode());

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