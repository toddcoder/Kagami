using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class ComparisandBooleanParser : SymbolParser
{
   public ComparisandBooleanParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(==|!=|<=|>=|<|>)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Operator);
      switch (tokens[2].Text)
      {
         case "==":
            builder.Add(new ComparisandEqualSymbol());
            break;
         case "!=":
            builder.Add(new ComparisandNotEqualSymbol());
            break;
         case "<=":
            builder.Add(new ComparisandLessThanEqualSymbol());
            break;
         case ">=":
            builder.Add(new ComparisandGreaterThanEqualSymbol());
            break;
         case "<":
            builder.Add(new ComparisandLessThanSymbol());
            break;
         case ">":
            builder.Add(new ComparisandGreaterThanSymbol());
            break;
         default:
            return nil;
      }

      return unit;
   }
}