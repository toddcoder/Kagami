using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class AssignToNewFieldParser : EndingInExpressionParser
   {
      bool mutable;
      string fieldName;

      public override string Pattern => $"^ /('let' | 'var') /(/s+) /({REGEX_FIELD}) /(/s+) /'='";

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         mutable = tokens[1].Text == "var";
         fieldName = tokens[3].Text;
         state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Structure);

         return Unit.Matched();
      }

      public override IMatched<Unit> Suffix(ParseState state, Expression expression)
      {
         state.AddStatement(new AssignToNewField(mutable, fieldName, expression));
         return Unit.Matched();
      }
   }
}