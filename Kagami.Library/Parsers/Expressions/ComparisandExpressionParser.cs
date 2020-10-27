using Core.Monads;
using Core.Numbers;
using Kagami.Library.Nodes.Symbols;

namespace Kagami.Library.Parsers.Expressions
{
   public class ComparisandExpressionParser : ExpressionParser
   {
      protected string fieldName;

      public ComparisandExpressionParser(Bits32<ExpressionFlags> flags, string fieldName) : base(flags | ExpressionFlags.Comparisand)
      {
         this.fieldName = fieldName;
      }

      protected override IMatched<Unit> getValue(ParseState state)
      {
         builder.Add(new FieldSymbol(fieldName));
         if (valuesParser.Scan(state).ValueOrOriginal(out _, out var original))
         {
            builder.Add(new MatchSymbol());
            return Unit.Matched();
         }
         else if (original.IsFailedMatch)
         {
            return valuesParser.Scan(state);
         }
         else
         {
            return "Invalid expression syntax".FailedMatch<Unit>();
         }
      }
   }
}