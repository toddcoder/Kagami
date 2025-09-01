using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class WithParser : SymbolParser
{
   public WithParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(with)(\{)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.OpenParenthesis);

      var _taggedExpressions = getTaggedExpressions(state, REGEX_BLOCK_END);
      if (_taggedExpressions is (true, var taggedExpressions))
      {
         builder.Add(new WithSymbol(taggedExpressions));
         return unit;
      }
      else
      {
         return _taggedExpressions.Exception;
      }
   }
}