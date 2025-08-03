using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Parsers.Expressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class DelegateParser(ClassBuilder builder) : StatementParser
{
   [GeneratedRegex($@"^(\s*)(delegate)(\s+)({REGEX_CLASS})(\s*)(=)(?![=>])")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var className = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class, Color.Whitespace, Color.Structure);

      var _constructor = getExpression(state, ExpressionFlags.Standard);
      if (_constructor is (true, var constructor))
      {

      }
      return unit;
   }
}