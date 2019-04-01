using Kagami.Library.Operations;
using Core.Monads;

namespace Kagami.Library.Nodes.Symbols
{
   public class SkipTakeItem
   {
      int skip;
      int take;
      IMaybe<Expression> prefix;
      IMaybe<Expression> suffix;

      public SkipTakeItem(int skip, int take, IMaybe<Expression> prefix, IMaybe<Expression> suffix)
      {
         this.skip = skip;
         this.take = take;
         this.prefix = prefix;
         this.suffix = suffix;
      }

      public void Generate(OperationsBuilder builder)
      {
         if (prefix.If(out var p))
         {
            p.Generate(builder);
            builder.Swap();
         }

         builder.PushInt(skip);
         builder.SendMessage("skip()", 1);

         if (take != 0)
         {
            builder.PushInt(take);
            builder.SendMessage("take()", 1);
         }

         if (prefix.IsSome)
            builder.SendMessage("~()", 1);

         if (suffix.If(out var s))
         {
            s.Generate(builder);
            builder.SendMessage("~()", 1);
         }
      }
   }
}