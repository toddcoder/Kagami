using Kagami.Library.Nodes.Symbols;
using Standard.Types.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class DefAssignParser : EndingInExpressionParser
   {
      string fieldName;

      public override string Pattern => $"^ /'def' /(|s+|) /({REGEX_FIELD}) /(|s|) /'=' -(> '=')";

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         fieldName = tokens[3].Text;
         state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Structure);

         return Unit.Matched();
      }

      public override IMatched<Unit> Suffix(ParseState state, Expression expression)
      {
         state.RegisterDefExpression(fieldName, expression);
         return Unit.Matched();
      }
   }
}