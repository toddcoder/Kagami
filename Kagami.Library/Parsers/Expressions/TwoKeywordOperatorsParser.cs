using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class TwoKeywordOperatorsParser : SymbolParser
{
   public TwoKeywordOperatorsParser(ExpressionBuilder builder) : base(builder)
   {
   }

   //public override string Pattern => "^ /(/s*) /('skip' | 'take') /(/s+) /('while' | 'until') /b";

   [GeneratedRegex(@"^(\s*)(skip|take)(\s+)(while|until)\b", RegexOptions.Compiled)]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.BeginTransaction();

      var word1 = tokens[2].Text;
      var word2 = tokens[4].Text;
      var message = "";
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Keyword);

      switch (word1)
      {
         case "skip":
         case "take":
            message = word2 switch
            {
               "while" or "until" => word1.Selector($"{word2}:"),
               _ => message
            };

            break;
         default:
            state.RollBackTransaction();
            return nil;
      }

      builder.Add(new SendBinaryMessageSymbol(message, Precedence.ChainedOperator));
      return unit;
   }
}