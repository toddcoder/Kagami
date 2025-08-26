using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.Objects;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class DollarFieldParser : SymbolParser
{
   public DollarFieldParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(\$)(\d+)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var index = tokens[3].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Identifier, Color.Identifier);

      builder.Add(new WhateverSymbol(index.Value().Int32()));
      return unit;
   }
}