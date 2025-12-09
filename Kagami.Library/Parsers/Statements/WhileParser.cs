using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class WhileParser : StatementParser
{
   protected Maybe<Statement> _statement = nil;

   [GeneratedRegex(@"^(\s*)(while)(?![>\^])\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      var isWhile = !state.NotKeyword();

      var _result =
         from expression in getExpression(state, ExpressionFlags.Standard)
         from _ in getIncrement(state)
         from block in getBlock(state)
         from exitedBlock in getExitBlock(state)
         select new While(expression, block, isWhile, exitedBlock.Maybe());
      if (_result is (true, var statement))
      {
         state.AddStatement(statement);
         if (_statement is (true, var lastStatement))
         {
            statement.AddIncrementerToBlock(lastStatement);
         }

         return unit;
      }
      else
      {
         return _result.Exception;
      }
   }

   protected Optional<Unit> getIncrement(ParseState state)
   {
      var _scan = state.Scan(@"^(\s*)(;)", Color.Whitespace, Color.Structure);
      if (_scan)
      {
         var statementsParser = new StatementsParser();
         var _result = statementsParser.Scan(state);
         if (_result)
         {
            _statement = state.PopStatement();
         }
      }

      return unit;
   }
}