using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class RefParser : SymbolParser
   {
      public RefParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => $"^ /(|s|) /'ref' /(|s+|) /({REGEX_FIELD}) /b";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var fieldName = tokens[4].Text;
         state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Whitespace, Color.Identifier);

         builder.Add(new CallSysFunctionSymbol0(s => s.GetReference(fieldName), $"ref {fieldName}"));
         return Unit.Matched();
      }
   }
}