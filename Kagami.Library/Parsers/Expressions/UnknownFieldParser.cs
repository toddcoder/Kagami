using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class UnknownFieldParser : SymbolParser
{
   public UnknownFieldParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override string Pattern => "^ /(|s|) /('$' /d+) /b";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var source = tokens[2].Text.Drop(1);
      state.Colorize(tokens, Color.Whitespace, Color.Identifier);

      Index = source.Value().Int32();
      builder.Add(new FieldSymbol($"__${source}"));

      return unit;
   }

   public int Index { get; set; } = -1;
}