﻿using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class BooleanParser : SymbolParser
{
   public BooleanParser(ExpressionBuilder builder) : base(builder)
   {
   }

   //public override string Pattern => "^ /(/s*) /('true' | 'false') /b";

   [GeneratedRegex(@"^ /(/s*) /('true' | 'false') /b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Boolean);
      builder.Add(new BooleanSymbol(tokens[2].Text == "true"));

      return unit;
   }
}