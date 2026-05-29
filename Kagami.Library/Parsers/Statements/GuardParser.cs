using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class GuardParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(subtype)(\s+)({REGEX_CLASS})(\()")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var guardName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class, Color.OpenParenthesis);

      var _parameters = getParameters(state);
      if (_parameters is (true, var parameters))
      {
         if (parameters.Length != 1)
         {
            return fail("Subtype must have exactly one parameter");
         }

         state.CreateReturnType();
         state.CreateYieldFlag();

         var _block =
            from scanned in state.Scan(@"^(\s*)(=)")
            from blockValue in getAnyBlock(state)
            select blockValue;
         if (_block is (true, var block))
         {
            state.RemoveReturnType();
            state.RemoveYieldFlag();

            block.AddReturnIf();
            Guards.Subtype.Set(guardName, new LambdaSymbol(parameters, block), parameters[0].TypeConstraint);

            return unit;
         }
         else
         {
            return _block.Exception;
         }
      }
      else
      {
         return _parameters.Exception;
      }
   }
}