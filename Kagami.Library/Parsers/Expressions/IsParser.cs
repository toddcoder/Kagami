using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Expressions;

public partial class IsParser : SymbolParser
{
   public IsParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(is)(\s+)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.BeginTransaction();
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace);

      var not = state.NotKeyword();

      var _result = getExpression(state, builder.Flags | ExpressionFlags.Comparisand);
      if (_result is (true, var comparisand))
      {
         var _fieldResult = state.Scan($@"^(\s+)({REGEX_FIELD})\b", Color.Whitespace, Color.Identifier);
         if (_fieldResult is (true, var fieldName))
         {
            var innerBuilder = new ExpressionBuilder(ExpressionFlags.Comparisand);
            innerBuilder.Add(new NameValueSymbol(fieldName.Trim(), comparisand));
            var _innerExpression = innerBuilder.ToExpression();
            if (_innerExpression is (true, var expression))
            {
               comparisand = expression;
            }
            else
            {
               state.RollBackTransaction();
               return _innerExpression.Exception;
            }
         }

         builder.Add(new IsSymbol(comparisand, not));
         state.CommitTransaction();

         return unit;
      }
      else if (_result.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         state.RollBackTransaction();
         return nil;
      }
   }
}