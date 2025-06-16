using Kagami.Library.Operations;
using System.Linq.Expressions;
using SkipTake = Kagami.Library.Parsers.Expressions.SkipTake;

namespace Kagami.Library.Nodes.Symbols;

public class SliceAssignSymbol : Symbol, IHasExpression
{
   protected SkipTake skipTake;
   protected Expression values;

   public SliceAssignSymbol(SkipTake skipTake, Expression values)
   {
      this.skipTake = skipTake;
      this.values = values;
   }

   public override void Generate(OperationsBuilder builder)
   {
      generateSkipTake(builder);
      values.Generate(builder);

      builder.SendMessage("assign(_,_)", 2);
   }

   protected void generateSkipTake(OperationsBuilder builder)
   {
      if (skipTake.Skip is (true, var skipExpression))
      {
         skipExpression.Generate(builder);
      }
      else
      {
         builder.PushInt(0);
      }

      if (skipTake.Take is (true, var takeExpression))
      {
         takeExpression.Generate(builder);
      }
      else
      {
         builder.PushInt(0);
      }

      builder.NewSkipTake();
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"{{{skipTake}}} = {values}";

   public Expression Expression => values;
}