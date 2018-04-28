using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class IfParser : ExpressionBlockParser
   {
      bool mutable;
      string fieldName;
      bool assignment;

      public override string Pattern => $"^ (/('var' | 'let') /(|s|) /({REGEX_FIELD}) /(|s|) /'=' /(|s|))? /'if' /b";

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         mutable = tokens[1].Text == "var";
         fieldName = tokens[3].Text;
         assignment = fieldName.IsNotEmpty();
         state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Structure,
            Color.Whitespace, Color.Keyword);

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

         state.AddStatement(new If(expression, block, elseIf, elseBlock, fieldName, mutable, assignment, true));
         return Unit.Matched();
      }
   }
}