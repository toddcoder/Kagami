using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Expressions;

public partial class SelectorParser : SymbolParser
{
   [GeneratedRegex($@"^(\s*)(&)({REGEX_FUNCTION_NAME})(\([^\)]*\))?")]
   public override partial Regex Regex();

   public SelectorParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      Selector selector = tokens[3].Text + tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Selector, Color.Selector, Color.Selector);

      if (state.Scan(@"^(\()", Color.OpenParenthesis))
      {
         var _arguments = getArgumentsPlusLambda(state, builder.Flags);
         if (_arguments is (true, var (arguments, _lambdaSymbol)))
         {
         }
         else if (_arguments.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            arguments = [];
            _lambdaSymbol = nil;
         }

         builder.Add(new InvokeSymbol(selector.Name, arguments, _lambdaSymbol, false));
      }
      else
      {
         builder.Add(new SelectorSymbol(selector));
      }

      return unit;
   }
}