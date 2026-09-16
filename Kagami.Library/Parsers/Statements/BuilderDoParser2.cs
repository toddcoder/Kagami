using Core.Monads;
using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class BuilderDoParser2(BuilderState builderState) : StatementParser
{
   [GeneratedRegex(@"^(\s*)(do)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      var parser = new ExpressionStatementParser(false, nil);
      state.PushStatements();
      var _scanned = parser.Scan(state);
      if (_scanned)
      {
         var _statements = state.PopStatements();
         if (_statements is (true, var statements))
         {
            Block block = [.. statements];
            state.AddStatement(new BuilderDo(builderState, block));

            return unit;
         }
         else
         {
            return _statements.Exception;
         }
      }
      else
      {
         return _scanned.Exception;
      }
   }
}