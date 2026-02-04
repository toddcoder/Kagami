using System.Text.RegularExpressions;
using Core.Monads;
using Core.Strings;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class LazyAssignParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*){REGEX_HIDDEN}{REGEX_OVERRIDE}(lazy)(\s+)({REGEX_FIELD})(\s*)(=)")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var isHidden = tokens[2].Text.IsNotEmpty();
      var isOverride = tokens[3].Text.IsNotEmpty();
      var fieldName = tokens[6].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Keyword, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Structure);

      var _expression = getExpression(state, ExpressionFlags.Standard);
      if (_expression is (true, var expression))
      {
         var block = new Block(expression);
         state.AddStatement(new LazyAssign(fieldName, block, isHidden, isOverride));
         return unit;
      }
      else if (_expression.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return fail("Expression missing");
      }
   }
}