using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class PipeToFieldParser : SymbolParser
{
   public PipeToFieldParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@$"^(\s*)(\|>)(\s*)({REGEX_FIELD})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var fieldName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Whitespace, Color.Identifier);

      builder.Add(new PipeToFieldSymbol(fieldName));
      return unit;
   }
}