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

   [GeneratedRegex(@"^(\s*)(skip|take|if|not|sort)(\s+)(while|until|not|same|desc)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.BeginTransaction();

      var word1 = tokens[2].Text;
      var word2 = tokens[4].Text;
      var message = "";
      state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Whitespace, Color.Operator);

      switch (word1)
      {
         case "skip":
         case "take":
            message = word2 switch
            {
               "while" or "until" => $"{word1}{word2.ToTitleCase()}".Selector(1),
               _ => message
            };

            break;
         case "if" when word2 == "not" && !builder.Flags[ExpressionFlags.OmitIf]:
            message = "ifNot(_)";
            break;
         case "not" when word2 == "same":
            builder.Add(new SameSymbol(true));
            return unit;
         case "sort" when word2 == "desc":
            message = "sortDesc(_<Lambda>)";
            break;
         default:
            state.RollBackTransaction();
            return nil;
      }

      builder.Add(new SendBinaryMessageSymbol(message, Precedence.ChainedOperator));
      return unit;
   }
}