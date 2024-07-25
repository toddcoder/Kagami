using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class SelectorParser : SymbolParser
   {
      public override string Pattern => $"^ /(|s|) /'&' /({REGEX_FUNCTION_NAME}) /('(' -[')']+ ')')?";

      public SelectorParser(ExpressionBuilder builder) : base(builder) { }

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
	      Selector selector = tokens[3].Text + tokens[4].Text;
	      state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Invokable, Color.Structure);
			builder.Add(new FieldSymbol(selector));

         return Unit.Matched();
      }
   }
}