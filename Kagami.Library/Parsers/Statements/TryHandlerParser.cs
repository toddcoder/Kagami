using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class TryHandlerParser : StatementParser
{
   [GeneratedRegex(@"^(\s*)(try)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.BeginTransaction();
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      var _result =
         from _tryBlock in getBlock(state)
         from catchKeyword in state.Scan(@"^(\s*)(catch)", Color.Whitespace, Color.Keyword)
         from _errorField in state.OptionalScan(@$"^(\s+)({REGEX_FIELD})", Color.Whitespace, Color.Identifier)
         from _catchBlock in getBlock(state)
         select (_tryBlock, _catchBlock, _errorField);
      if (_result is (true, var (tryBlock, catchBlock, errorField)))
      {
         var _finally =
            from finallyKeyword in state.OptionalScan(@"^(\s*)(finally)", Color.Whitespace, Color.Keyword).Maybe()
            from finallyBlock in getBlock(state)
            select finallyBlock;
         var _errorField = errorField.Maybe;
         state.AddStatement(new TryHandler(tryBlock, catchBlock, _errorField.Map(f => f.Trim()), _finally));
         state.CommitTransaction();

         return unit;
      }
      else
      {
         state.RollBackTransaction();
         return _result.Exception;
      }
   }
}