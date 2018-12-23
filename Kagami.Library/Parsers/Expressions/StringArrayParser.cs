using Kagami.Library.Nodes.Symbols;
using Standard.Types.Monads;

namespace Kagami.Library.Parsers.Expressions
{
   public class StringArrayParser : SymbolParser
   {
      public override string Pattern => "^ /(|s|) /'a\"' /(-['\"']*) /'\"'";

      public StringArrayParser(ExpressionBuilder builder) : base(builder) { }

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var source = tokens[3].Text;
         state.Colorize(tokens, Color.Whitespace, Color.Collection, Color.Collection, Color.Collection);

         builder.Add(new StringArraySymbol(source));

         return Unit.Matched();
      }
   }
}