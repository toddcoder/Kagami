using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public class AliasParser : StatementParser
{
   public override string Pattern => $"^ /'alias' /(|s+|) /({REGEX_CLASS}) /(|s|) /'=' /(|s|) /({REGEX_CLASS}) {REGEX_ANTICIPATE_END}";

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var aliasName = tokens[3].Text;
      var className = tokens[7].Text;
      state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class, Color.Whitespace, Color.Structure, Color.Whitespace, Color.Class);

      var _alias = Module.Global.Alias(aliasName, className);
      if (_alias)
      {
         return unit;
      }
      else
      {
         return _alias.Exception;
      }
   }
}