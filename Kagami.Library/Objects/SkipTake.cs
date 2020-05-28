using Core.Collections;
using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects
{
   public class SkipTake : IObject
   {
      IObject source;
      IMaybe<IObject> result;

      public SkipTake(IObject source)
      {
         this.source = source;
         result = none<IObject>();
      }

      public string ClassName => "SkipTake";

      public string AsString => $"{source.AsString}{result.Map(o => o.AsString).DefaultTo(() => "")}";

      public string Image => $"{source.Image}{result.Map(o => o.Image).DefaultTo(() => "")}";

      public int Hash
      {
         get
         {
            var hash = 17;
            hash = 37 * source.Hash;
            hash = 37 * result.Map(o => o.Hash).DefaultTo(() => 0);

            return hash;
         }
      }

      public bool IsEqualTo(IObject obj)
      {
         return obj is SkipTake st && source.IsEqualTo(st.source) &&
            result.DefaultTo(() => None.NoneValue).IsEqualTo(st.result.DefaultTo(() => None.NoneValue));
      }

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => result.IsSome;

      public SkipTake Literal(IObject literal)
      {
         if (result.If(out var r))
         {
            result = sendMessage(r, "~(_)", literal).Some();
         }
         else
         {
            result = literal.Some();
         }

         return this;
      }

      public SkipTake Skip(int count)
      {
         source = sendMessage(source, "skip(_)", new Int(count));
         return this;
      }

      public SkipTake Take(int count)
      {
         var taken = sendMessage(source, "take(_)", new Int(count));
         Literal(taken);
         Skip(count);

         return this;
      }

      public SkipTake TakeRest()
      {
         var count = ((Int)sendMessage(source, "length".get())).Value;
         return Take(count);
      }

      public IObject FullResult => result.DefaultTo(() => source);
   }
}