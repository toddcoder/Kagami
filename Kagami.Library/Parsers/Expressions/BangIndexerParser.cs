using System.Text.RegularExpressions;
using Core.Monads;
using Core.Strings;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class BangIndexerParser : SymbolParser
{
   public BangIndexerParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@$"^(\!)({REGEX_FIELD})")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var key = tokens[2].Text;
      state.Colorize(tokens, Color.Operator, Color.Label);

      var keyExpression = new Expression(new StringSymbol(key));
      var _scan = state.Scan(@$"^(\s*)({REGEX_ASSIGN_OPS})?(=)(?!=)", Color.Whitespace, Color.Operator, Color.Structure);
      if (_scan is (true, var opSource))
      {
         var _expression = getExpression(state, builder.Flags);
         if (_expression is (true, var expression))
         {
            opSource = opSource.DropWhile(" ").Keep(1);
            var operation = matchOperator(opSource) | nil;
            builder.Add(new IndexSetterSymbol([keyExpression], expression, operation.Maybe()));

            return unit;
         }
         else
         {
            return _expression.Exception;
         }
      }
      else if (_scan.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         builder.Add(new IndexerSymbol([keyExpression]));
      }

      return unit;
   }
}