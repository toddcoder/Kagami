using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class TwoKeywordOperatorsParser : SymbolParser
{
   public TwoKeywordOperatorsParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(skip|take|if|not|sort|map|group|flat)(\s+)(while|until|not|same|desc|if|by|map)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.BeginTransaction();

      var word1 = tokens[2].Text;
      var word2 = tokens[4].Text;
      string message;
      state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Whitespace, Color.Operator);

      switch (word1, word2)
      {
         case ("skip", "while"):
         case ("skip", "until"):
         case ("take", "while"):
         case ("take", "until"):
            message = $"{word1}{word2.ToTitleCase()}".Selector(1);
            break;
         case ("if", "not") when !builder.Flags[ExpressionFlags.OmitIf]:
            message = "ifNot(_)";
            break;
         case ("not", "same"):
            builder.Add(new SameSymbol(true));
            return unit;
         case ("sort", "desc"):
            message = "sortDesc(_<Lambda>)";
            break;
         case ("map", "if") when !builder.Flags[ExpressionFlags.OmitIf]:
            message = "mapIf(_<Lambda>)";
            break;
         case ("group", "by"):
            message = "groupBy(_<Lambda>)";
            break;
         case ("flat", "map"):
            message = "flatMap(_<Lambda>)";
            break;
         default:
            state.RollBackTransaction();
            return nil;
      }

      builder.Add(new SendBinaryMessageSymbol(message, Precedence.ChainedOperator));
      return unit;
   }
}