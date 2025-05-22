using Core.Monads;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public class PatternParser : StatementParser
{
   public override string Pattern => $"^ /'pattern' /(/s+) /({REGEX_CLASS}) /'('";

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var name = tokens[3].Text;
      state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class, Color.OpenParenthesis);
      state.CreateReturnType();

      var _result =
         from parametersValue in getParameters(state)
         from blockValue in getAnyBlock(state)
         select (parametersValue, blockValue);
      if (_result is (true, var (parameters, block)))
      {
         state.RemoveReturnType();
         state.RegisterPattern(name);
         state.AddStatement(new Pattern(name, parameters, block));

         return unit;
      }
      else
      {
         state.RemoveReturnType();
         return _result.Exception;
      }
   }
}