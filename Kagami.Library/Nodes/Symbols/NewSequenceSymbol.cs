using Core.Enumerables;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class NewSequenceSymbol(Expression[] expressions) : Symbol, IHasExpressions
{
   public override void Generate(OperationsBuilder builder)
   {
      if (expressions.Length > 1)
      {
         var firstTwo = expressions.Take(2).ToList();
         firstTwo[0].Generate(builder);
         firstTwo[1].Generate(builder);
         builder.NewSequence();

         foreach (var expression in expressions.Skip(2))
         {
            expression.Generate(builder);
            builder.NewSequence();
         }
      }
      else
      {
         expressions[0].Generate(builder);
         IsOpenRange = expressions[0].Symbols.LastOrNone(s => s is OpenRangeSymbol);
      }
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public bool IsOpenRange { get; set; }

   public Expression[] Expressions => expressions;
}