using Core.Enumerables;
using Kagami.Library.Operations;
using Kagami.Library.Parsers.Expressions;

namespace Kagami.Library.Nodes.Symbols;

public class SliceSymbol : Symbol
{
   protected SliceParser.SkipTake[] skipTakes;

   public SliceSymbol(SliceParser.SkipTake[] skipTakes) => this.skipTakes = skipTakes;

   public override void Generate(OperationsBuilder builder)
   {
      builder.PushFrameWithValue();

      var firstSkipTake = false;
      foreach (var skipTake in skipTakes)
      {
         if (firstSkipTake)
         {
            builder.Copy(1);
         }
         else
         {
            builder.Dup();
         }

         generate(builder, skipTake);
         if (firstSkipTake)
         {
            builder.SendMessage("~(_)", 1);
         }

         firstSkipTake = true;
      }

      builder.PopFrameWithValue();
   }

   protected static void generate(OperationsBuilder builder, SliceParser.SkipTake _skipTake)
   {
      if (_skipTake.Skip is (true, var skipExpression))
      {
         skipExpression.Generate(builder);
         builder.SendMessage("skip(_)", 1);
      }

      if (_skipTake.Take is (true, var takeExpression))
      {
         takeExpression.Generate(builder);
         builder.SendMessage("take(_)", 1);
      }
   }

   public override Precedence Precedence => Precedence.SendMessage;

   public override Arity Arity => Arity.Postfix;

   public override string ToString() => $"{{{skipTakes.ToString(", ")}}}";
}