using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class ReassignmentParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)({REGEX_FIELD})(\s*)(\.=)")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var fieldName = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Structure);

      var _expression = getExpression(state, ExpressionFlags.Standard);
      if (_expression is (true, var expression))
      {
         state.AddStatement(new Reassignment(fieldName, expression));
         return unit;
      }
      else if (_expression.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return nil;
      }
   }
}