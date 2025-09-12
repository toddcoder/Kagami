using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class MatchExpressionParser : SymbolParser
{
   protected static Optional<(Expression, Expression)> getMatchItem(ParseState state)
   {
      if (state.Scan(@"^(\s*)(\})", Color.Whitespace, Color.CloseParenthesis))
      {
         return nil;
      }
      else
      {
         var _matchItem =
            from key in getExpression(state, ExpressionFlags.Comparisand | ExpressionFlags.OmitNameValue)
            from _ in state.Scan($@"^(\s*){REGEX_SINGLE_BLOCK}", Color.Whitespace, Color.Operator)
            from expression in getExpression(state, ExpressionFlags.Standard)
            select (key, expression);
         if (_matchItem)
         {
            state.Scan(@"^(\s*)(;)", Color.Whitespace, Color.Structure);
         }

         return _matchItem;
      }
   }

   public MatchExpressionParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(match)(\s*)(\{)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Whitespace, Color.OpenParenthesis);

      List<(Expression, Expression)> matchItems = [];

      while (state.More)
      {
         var _matchItem = getMatchItem(state);
         if (_matchItem is (true, var matchItem))
         {
            matchItems.Add(matchItem);
         }
         else if (_matchItem.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            break;
         }
      }

      builder.Add(new MatchExpressionSymbol([.. matchItems]));
      return unit;
   }
}