using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class ConditionalAssignParser : StatementParser
{
   //public override string Pattern => "^ /'if' /(/s+)";

   [GeneratedRegex(@"^(if)(\s+)")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.BeginTransaction();

      state.Colorize(tokens, Color.Keyword, Color.Whitespace);

      var _result =
         from comparisandValue in getExpression(state, ExpressionFlags.Comparisand)
         from scanned in state.Scan(@"^(\s*)(:=)", Color.Whitespace, Color.Structure)
         from expressionValue in getExpression(state, ExpressionFlags.Standard)
         from andValue in getAnd(state)
         from blockValue in getBlock(state)
         select (comparisandValue, expressionValue, andValue, blockValue);

      if (_result is (true, var (comparisand, expression, _and, block)))
      {
         Maybe<Block> _elseBlock = nil;
         var elseParser = new ElseParser();
         var _scan = elseParser.Scan(state);
         if (_scan)
         {
            _elseBlock = elseParser.Block;
         }
         else if (_scan.Exception is (true, var exception))
         {
            state.RollBackTransaction();
            return exception;
         }

         if (_and is (true, var and))
         {
            var builder = new ExpressionBuilder(ExpressionFlags.Comparisand);
            builder.Add(and);
            var _expression = builder.ToExpression();
            /*if (_expression.IfNot(out expression, out var exception))
            {
               state.RollBackTransaction();
               return failedMatch<Unit>(exception);
            }*/
            if (_expression)
            {
               expression = _expression;
            }
            else
            {
               state.RollBackTransaction();
               return _expression.Exception;
            }
         }

         state.CommitTransaction();
         state.AddStatement(new ConditionalAssign(comparisand, expression, block, _elseBlock));

         return unit;
      }
      else
      {
         state.RollBackTransaction();
         return _result.Exception;
      }
   }
}