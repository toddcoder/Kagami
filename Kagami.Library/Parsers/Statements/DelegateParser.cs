using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class DelegateParser(ClassBuilder classBuilder) : StatementParser
{
   [GeneratedRegex(@"^(\s*)(delegate)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      var _constructor = getExpression(state, ExpressionFlags.Standard);
      if (_constructor is (true, var constructor))
      {
         if (constructor.Symbols.Length == 0)
         {
            return new Exception($"No symbols found in constructor expression: {constructor}");
         }
         else if (constructor.Symbols[0] is InvokeSymbol invoke)
         {
            classBuilder.RegisterDelegate(invoke.FunctionName, constructor);
            return unit;
         }
         else
         {
            return nil;
         }
      }
      else
      {
         return _constructor.Exception;
      }
   }
}