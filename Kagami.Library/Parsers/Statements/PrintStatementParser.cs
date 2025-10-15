using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class PrintStatementParser : StatementParser
{
   [GeneratedRegex(@"^(\s*)(println|print|put)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      if (!state.AllowPrintStatement)
      {
         return nil;
      }

      Maybe<PrintStatementType> _type = tokens[2].Text switch
      {
         "println" => PrintStatementType.Println,
         "print" => PrintStatementType.Print,
         "put" => PrintStatementType.Put,
         _ => nil
      };
      if (_type is (true, var type))
      {
         state.Colorize(tokens, Color.Whitespace, Color.Keyword);
         var _expression = getExpression(state, ExpressionFlags.Standard);
         if (_expression is (true, var expression))
         {
            state.AddStatement(new PrintStatement(type, expression));
            return unit;
         }
         else
         {
            return _expression.Exception;
         }
      }
      else
      {
         return fail("Didn't recognize print type");
      }
   }
}