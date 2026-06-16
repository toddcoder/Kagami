using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class CoverParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(cover)(\s+)({REGEX_FIELD})(\s+)(in)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var identifier = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Keyword);

      var _expression = getExpression(state, ExpressionFlags.Standard);
      if (_expression is (true, var expression))
      {
         var _first = getBlock(state, "first");
         var _middle = getBlock(state, "middle");
         var _last = getBlock(state, "last");

         state.AddStatement(new CoverStatement(expression, identifier, _first, _middle, _last));
         return unit;
      }
      else
      {
         return _expression.Exception;
      }
   }

   protected static Maybe<Block> getBlock(ParseState state, string keyword)
   {
      var _scan = state.Scan($@"^(\s*)({keyword})\b", Color.Whitespace, Color.Keyword);
      if (_scan)
      {
         return ParserFunctions.getBlock(state).Maybe();
      }
      else
      {
         return nil;
      }
   }
}