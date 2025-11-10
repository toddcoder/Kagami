using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class SingletonParser : StatementParser
{
   [GeneratedRegex($@"^(\s*)(singleton)(\s+)({REGEX_FIELD})")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var singletonName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier);

      state.CreateYieldFlag();
      state.CreateReturnType();

      var _block = getAnyBlock(state);
      if (_block is (true, var block))
      {
         state.RemoveYieldFlag();
         var _typeConstraint = state.GetReturnType();
         if (_typeConstraint is (true, var typeConstraint))
         {
            state.AddStatement(new Singleton(singletonName, block, typeConstraint));
            return unit;
         }
         else
         {
            return fail("Type constraint required");
         }
      }
      else
      {
         return _block.Exception;
      }
   }
}