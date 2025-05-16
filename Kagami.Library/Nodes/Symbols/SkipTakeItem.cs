using Kagami.Library.Operations;
using Core.Monads;

namespace Kagami.Library.Nodes.Symbols;

public class SkipTakeItem
{
   protected int skip;
   protected int take;
   protected Maybe<Expression> _prefix;
   protected Maybe<Expression> _suffix;

   public SkipTakeItem(int skip, int take, Maybe<Expression> _prefix, Maybe<Expression> _suffix)
   {
      this.skip = skip;
      this.take = take;
      this._prefix = _prefix;
      this._suffix = _suffix;
   }

   public void Generate(OperationsBuilder builder)
   {
      if (_prefix is (true, var prefix))
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

      if (_prefix)
      {
         builder.SendMessage("~()", 1);
      }

      if (_suffix is (true, var suffix))
      {
         suffix.Generate(builder);
         builder.SendMessage("~()", 1);
      }
   }
}