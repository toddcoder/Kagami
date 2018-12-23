using Kagami.Library.Nodes.Symbols;
using Standard.Types.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class BindingParser : EndingInValueParser
   {
      string name;

      public BindingParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => $"^ /(|s|) /({REGEX_FIELD}) /(|s|) /'@'";

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         name = tokens[2].Text;
         state.Colorize(tokens, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Operator);

         return Unit.Matched();
      }

      public override IMatched<Unit> Suffix(ParseState state, Symbol value)
      {
         builder.Add(new BindingSymbol(name, value));
         return Unit.Matched();
      }
   }
}