using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Core.Monads;
using Kagami.Library.Parsers.Expressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class ElseIfParser : StatementParser
{
   protected string fieldName;
   protected bool mutable;
   protected bool assignment;

   public ElseIfParser(string fieldName, bool mutable, bool assignment)
   {
      this.fieldName = fieldName;
      this.mutable = mutable;
      this.assignment = assignment;
   }

   [GeneratedRegex(@"^(\s*)(else)(\s+)(if)\b")]
   public override partial Regex Regex();

   public Maybe<If> If { get; set; } = nil;

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Keyword);

      var not = state.NotKeyword();

      var _result =
         from expressionValue in getExpression(state, ExpressionFlags.OmitIf)
         from blockValue in getBlock(state)
         select (expressionValue, blockValue);
      if (_result is (true, var (expression, block)))
      {
         Maybe<If> _elseIf = nil;
         var elseIfParser = new ElseIfParser(fieldName, mutable, assignment);

         var _scan = elseIfParser.Scan(state);
         if (_scan)
         {
            _elseIf = elseIfParser.If;
         }
         else if (_scan.Exception is (true, var exception))
         {
            return exception;
         }

         Maybe<Block> _elseBlock = nil;
         var elseParser = new ElseParser();
         _scan = elseParser.Scan(state);
         if (_scan)
         {
            _elseBlock = elseParser.Block;
         }
         else if (_scan.Exception is (true, var exception))
         {
            return exception;
         }

         If = new If(expression, not, block, _elseIf, _elseBlock, fieldName, mutable, assignment, false);
         return unit;
      }
      else
      {
         return _result.Exception;
      }
   }
}