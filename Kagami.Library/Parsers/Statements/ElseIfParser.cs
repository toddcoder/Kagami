using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class ElseIfParser : ExpressionBlockParser
   {
      string fieldName;
      bool mutable;
      bool assignment;

      public ElseIfParser(string fieldName, bool mutable, bool assignment)
      {
         this.fieldName = fieldName;
         this.mutable = mutable;
         this.assignment = assignment;
      }

      public override string Pattern => "^ /'else' /(|s+|) /'if' /b";

      public IMaybe<If> If { get; set; } = none<If>();

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Keyword);

         return Unit.Matched();
      }

      public override IMatched<Unit> Suffix(ParseState state, Expression expression, Block block)
      {
         var elseIf = none<If>();
         var elseIfParser = new ElseIfParser(fieldName, mutable, assignment);
         (var elseIfType, _, var elseIfException) = elseIfParser.Scan(state).Values;
         switch (elseIfType)
         {
            case MatchType.Matched:
               elseIf = elseIfParser.If;
               break;
            case MatchType.FailedMatch:
               return failedMatch<Unit>(elseIfException);
         }

         var elseBlock = none<Block>();
         var elsePaser = new ElseParser();
         (var elseType, _, var elseException) = elsePaser.Scan(state).Values;
         switch (elseType)
         {
            case MatchType.Matched:
               elseBlock = elsePaser.Block;
               break;
            case MatchType.FailedMatch:
               return failedMatch<Unit>(elseException);
         }

         If = new If(expression, block, elseIf, elseBlock, fieldName, mutable, assignment, false).Some();

         return Unit.Matched();
      }
   }
}