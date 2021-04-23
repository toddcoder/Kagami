using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class ElseIfParser : ExpressionBlockParser
   {
      protected string fieldName;
      protected bool mutable;
      protected bool assignment;

      public ElseIfParser(string fieldName, bool mutable, bool assignment)
      {
         this.fieldName = fieldName;
         this.mutable = mutable;
         this.assignment = assignment;
         If = none<If>();
      }

      public override string Pattern => "^ /'else' /(|s+|) /'if' /b";

      public IMaybe<If> If { get; set; }

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Keyword);
         return Unit.Matched();
      }

      public override IMatched<Unit> Suffix(ParseState state, Expression expression, Block block)
      {
         var elseIf = none<If>();
         var elseIfParser = new ElseIfParser(fieldName, mutable, assignment);

         if (elseIfParser.Scan(state).If(out _, out var _exception))
         {
            elseIf = elseIfParser.If;
         }
         else if (_exception.If(out var exception))
         {
            return failedMatch<Unit>(exception);
         }

         var elseBlock = none<Block>();
         var elseParser = new ElseParser();
         if (elseParser.Scan(state).If(out _, out _exception))
         {
            elseBlock = elseParser.Block;
         }
         else if (_exception.If(out var exception))
         {
            return failedMatch<Unit>(exception);
         }

         If = new If(expression, block, elseIf, elseBlock, fieldName, mutable, assignment, false).Some();
         return Unit.Matched();
      }
   }
}