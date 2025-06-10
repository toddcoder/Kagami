using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Expressions;

public partial class ImplicitCollectionExpressionParser(ExpressionBuilder builder) : SymbolParser(builder)
{
   [GeneratedRegex(@"^(\s+)(map|if|foldl|foldr)(>)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var message = tokens[2].Text;
      var unknownCount = message switch
      {
         "map" => 1,
         "if" => 1,
         "foldl" => 2,
         "foldr" => 2,
         _ => 0
      };
      var selector = message.Selector(unknownCount);
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Keyword);

      var _expression = getExpression(state, builder.Flags);
      if (_expression is (true, var expression))
      {
         var expressionStatement = new ExpressionStatement(expression, true);
         var block = new Block(expressionStatement);
         var lambdaSymbol = new LambdaSymbol(unknownCount, block);

         builder.Add(new SendMessageSymbol(selector, lambdaSymbol));

         return unit;
      }
      else
      {
         return _expression.Exception;
      }
   }
}