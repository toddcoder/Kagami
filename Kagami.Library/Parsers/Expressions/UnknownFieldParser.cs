using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using Standard.Types.Strings;

namespace Kagami.Library.Parsers.Expressions
{
   public class UnknownFieldParser : SymbolParser
   {
      public UnknownFieldParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /('$' /d+) /b";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var source = tokens[2].Text.Skip(1);
         state.Colorize(tokens, Color.Whitespace, Color.Identifier);

         Index = source.ToInt();
         builder.Add(new FieldSymbol(source.get()));

         return Unit.Matched();
      }

      public int Index { get; set; } = -1;
   }
}