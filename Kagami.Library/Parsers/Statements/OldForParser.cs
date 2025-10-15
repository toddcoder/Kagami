using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using System.Text.RegularExpressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class OldForParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(for)(\s+)({REGEX_FIELD})(\s*)(=)")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.BeginTransaction();

      var fieldName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Structure);
      var _result =
         from initializerValue in getExpression(state, ExpressionFlags.Standard)
         from semiColon1 in state.Scan(@"(\s*)(;)", Color.Whitespace, Color.Structure)
         from conditionValue in getExpression(state, ExpressionFlags.Standard)
         from semiColon2 in state.Scan(@"(\s*)(;)", Color.Whitespace, Color.Structure)
         from incrementValue in getIncrement(state)
         select (initializerValue, conditionValue, incrementValue);
      if (_result is (true, var (initializer, condition, increment)))
      {
         var _block = getBlock(state);
         if (_block is (true, var block))
         {
            var _exitBlock = getExitBlock(state);
            if (_exitBlock is (true, var exitBlock))
            {
               state.AddStatement(new OldFor(fieldName, initializer, condition, block, increment, exitBlock.Maybe()));
               state.CommitTransaction();
            }
            else
            {
               return _exitBlock.Exception;
            }

            return unit;
         }
         else
         {
            state.RollBackTransaction();
            return _block.Exception;
         }
      }
      else
      {
         state.RollBackTransaction();
         return _result.Exception;
      }
   }

   protected static Optional<Statement> getIncrement(ParseState state)
   {
      var statementsParser = new StatementsParser();
      var _result = statementsParser.Scan(state);
      if (_result)
      {
         return state.PopStatement().Optional();
      }
      else
      {
         return _result.Exception;
      }
   }
}