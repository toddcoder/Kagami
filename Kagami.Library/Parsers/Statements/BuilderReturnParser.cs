using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class BuilderReturnParser(BuilderState builderState) : StatementParser
{
   [GeneratedRegex(@"^(\s*)(return)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      var _expression = ParserFunctions.getExpression(state, ExpressionFlags.Standard);
      if (_expression is (true, var expression))
      {
         state.AddStatement(new BuilderReturn(builderState, expression));
      }
      else
      {
         return _expression.Exception;
      }

      return unit;
   }
}