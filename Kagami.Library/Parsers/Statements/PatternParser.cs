using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class PatternParser : StatementParser
{
   [GeneratedRegex($@"^(\s*)(pattern)(\s+)({REGEX_CLASS})(\()")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var name = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class, Color.OpenParenthesis);
      state.CreateReturnType();

      var _result =
         from parametersValue in getParameters(state)
         from blockValue in getAnyBlock(state)
         select (parametersValue, blockValue);
      if (_result is (true, var (parameters, block)))
      {
         state.RemoveReturnType();
         state.RegisterPattern(name);
         state.AddStatement(new PatternStatement(name, parameters, block));

         return unit;
      }
      else
      {
         state.RemoveReturnType();
         return _result.Exception;
      }
   }
}