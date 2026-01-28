using System.Text.RegularExpressions;
using Core.Monads;
using Core.Monads.Lazy;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class AnnotationParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(@)({REGEX_FUNCTION_NAME})(\()?")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var name = tokens[3].Text;
      var hasArguments = tokens[4].Text == "(";
      state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Class, Color.OpenParenthesis);

      Expression[] argumentsToUse = [];
      LazyOptional<Expression[]> _arguments = nil;
      if (hasArguments && _arguments.ValueOf(getArguments(state, ExpressionFlags.Standard)) is (true, var arguments))
      {
         argumentsToUse = arguments;
      }
      else if (_arguments.Exception is (true, var exception))
      {
         return exception;
      }

      var invokeSymbol = new InvokeSymbol(name, argumentsToUse, nil, false);
      state.PushAnnotation(invokeSymbol);

      return unit;
   }
}