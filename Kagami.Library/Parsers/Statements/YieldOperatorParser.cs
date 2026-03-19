using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Nodes.NodeFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class YieldOperatorParser : StatementParser
{
   [GeneratedRegex(@"^(\s*)(=|\*)(>)")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var all = tokens[2].Text == "*";
      state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Operator);

      var _expression = getExpression(state, ExpressionFlags.Standard);
      if (_expression is (true, var expression))
      {
         if (all)
         {
            var placeholderName = newLabel("yieldIndex");
            var block = new Block(new Yield(new Expression(new FieldSymbol(placeholderName))));
            var @for = new For(new PlaceholderSymbol("-" + placeholderName), expression, block, new PossibleIfExpression.None());
            state.AddStatement(@for);
         }
         else
         {
            state.AddStatement(new Yield(expression));
         }

         state.SetYieldFlag();
         return unit;
      }
      else
      {
         return _expression.Exception;
      }
   }
}