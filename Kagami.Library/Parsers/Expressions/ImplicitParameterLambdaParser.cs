using System.Text.RegularExpressions;
using Core.Collections;
using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.Expressions.ExpressionFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class ImplicitParameterLambdaParser : SymbolParser
{
   public ImplicitParameterLambdaParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(\(\|)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Lambda);
      var _expression = getExpression(state, REGEX_EXP_END, builder.Flags, Color.Whitespace, Color.Lambda);
      if (_expression is (true, var expression))
      {
         Set<string> whateverCount = [];
         evaluate(expression, s => s is DollarFieldSymbol, s =>
         {
            if (s is DollarFieldSymbol whateverSymbol)
            {
               whateverCount.Add(whateverSymbol.FieldName);
            }
         });
         var parameterNames = whateverCount.Order().Select(n => new Parameter(false, false, "", n, nil, nil, false, false, false));
         var parameters = new Parameters([ ..parameterNames]);
         var lambdaSymbol = new LambdaSymbol(parameters, expression);
         builder.Add(lambdaSymbol);

         return unit;
      }
      else
      {
         return _expression.Exception;
      }
   }
}