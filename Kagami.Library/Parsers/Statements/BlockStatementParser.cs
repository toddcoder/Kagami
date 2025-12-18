using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class BlockStatementParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(block)(\s+)({REGEX_FIELD})")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var fieldName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier);
      var _block = getBlock(state);
      if (_block is (true, var block))
      {
         var lambdaSymbol = new LambdaSymbol(0, block, false);
         state.AddStatement(new AssignDefinition(fieldName, lambdaSymbol));

         return unit;
      }
      else
      {
         return _block.Exception;
      }
   }
}