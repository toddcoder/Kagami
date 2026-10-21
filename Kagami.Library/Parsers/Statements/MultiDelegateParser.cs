using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class MultiDelegateParser : StatementParser
{
   [GeneratedRegex(@"^(\s*)(uses)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      List<(string functionName, Expression constructor)> uses = [];
      var scanComma = false;

      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      while (state.More)
      {
         if (scanComma)
         {
            var _comma = state.Scan(@"^(\s*)(,)", Color.Whitespace, Color.Structure);
            if (!_comma)
            {
               break;
            }
         }
         else
         {
            scanComma = true;
         }

         if (state.Scan(@"^(?=\r|\n|\s*;|\s*\{)"))
         {
            break;
         }

         var _constructor = getExpression(state, ExpressionFlags.OmitComma | ExpressionFlags.OmitFollowingBlock);
         if (_constructor is (true, var constructor))
         {
            if (constructor.Symbols.Length == 0)
            {
               return new Exception($"No symbols found in constructor expression: {constructor}");
            }
            else if (constructor.Symbols[0] is InvokeSymbol invoke)
            {
               uses.Add((invoke.FunctionName, constructor));
            }
            else
            {
               return nil;
            }
         }
         else if (_constructor.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            break;
         }
      }

      Uses = [.. uses];
      return unit;
   }

   public (string functionName, Expression constructor)[] Uses { get; set; } = [];
}