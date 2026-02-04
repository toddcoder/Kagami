using System.Text.RegularExpressions;
using Core.Monads;
using Core.Strings;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class SingletonParser : StatementParser
{
   [GeneratedRegex($@"^(\s*){REGEX_HIDDEN}{REGEX_OVERRIDE}(singleton)(\s+)({REGEX_FIELD})")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var isHidden = tokens[2].Text.IsNotEmpty();
      var isOverride = tokens[3].Text.IsNotEmpty();
      var singletonName = tokens[6].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Keyword, Color.Keyword, Color.Whitespace, Color.Identifier);

      state.CreateYieldFlag();
      state.CreateReturnType();

      var _block = getAnyBlock(state);
      if (_block is (true, var block))
      {
         state.RemoveYieldFlag();
         state.AddStatement(new LazyAssign(singletonName, block, isHidden, isOverride));

         return unit;
      }
      else
      {
         return _block.Exception;
      }
   }
}