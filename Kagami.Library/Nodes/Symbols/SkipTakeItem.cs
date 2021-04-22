using Kagami.Library.Operations;
using Core.Monads;

namespace Kagami.Library.Nodes.Symbols
{
   public class SkipTakeItem
   {
      protected int skip;
      protected int take;
      protected IMaybe<Expression> _prefix;
      protected IMaybe<Expression> _suffix;

      public SkipTakeItem(int skip, int take, IMaybe<Expression> prefix, IMaybe<Expression> suffix)
      {
         this.skip = skip;
         this.take = take;
         _prefix = prefix;
         _suffix = suffix;
      }

      public void Generate(OperationsBuilder builder)
      {
         if (_prefix.If(out var prefix))
         {
            prefix.Generate(builder);
            builder.Swap();
         }

         builder.PushInt(skip);
         builder.SendMessage("skip()", 1);

         if (take != 0)
         {
            builder.PushInt(take);
            builder.SendMessage("take()", 1);
         }

         if (_prefix.IsSome)
         {
            builder.SendMessage("~()", 1);
         }

         if (_suffix.If(out var suffix))
         {
            suffix.Generate(builder);
            builder.SendMessage("~()", 1);
         }
      }
   }
}