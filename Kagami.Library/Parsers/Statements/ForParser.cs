using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class ForParser : ExpressionBlockParser
   {
      string fieldName;

      public override string Pattern => $"^ /'for' /(|s|) /({REGEX_FIELD}) /(|s|) /'<-'";

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         fieldName = tokens[3].Text;
         state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Structure);

         return Unit.Matched();
      }

      public override IMatched<Unit> Suffix(ParseState state, Expression expression, Block block)
      {
         state.AddStatement(new For(fieldName, expression, block));
         return Unit.Matched();
      }
   }
}