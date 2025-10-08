using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class DecoratorParser : StatementParser
{
   protected const string REGEX_INVOKE = @$"^(\s*)({REGEX_FUNCTION_NAME})(\()";

   [GeneratedRegex(@"^(\s*)(\[)")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Structure);

      List<InvokeSymbol> invokeSymbols = [];

      while (state.More)
      {
         if (state.Scan(@"^(\s*)(\]"))
         {
            break;
         }

         var _result = state.Scan(REGEX_INVOKE, 2, Color.Whitespace, Color.Invokable);
         if (_result is (true, var invokableName))
         {
            var _arguments = getArguments(state, ExpressionFlags.InArgument);
            if (_arguments is (true, var arguments))
            {
               var invokeSymbol = new InvokeSymbol(invokableName, arguments, nil, false);
               invokeSymbols.Add(invokeSymbol);
            }
         }
      }

      return unit;
   }
}