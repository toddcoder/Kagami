﻿using System.Text.RegularExpressions;
using Core.Matching;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class FormatParser : SymbolParser
{
   [GeneratedRegex(@"^(\s*)(\$[cdefgnprxsboi](?:-?\d+)?(?:\.\d+)?)")]
   public override partial Regex Regex();

   public FormatParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var input = tokens[2].Text.Drop(1);
      if (!input.Contains("."))
      {
         if (input.IsMatch("^ ['efgnp']"))
         {
            input += ".6";
         }
         else if (input == "c")
         {
            input += ".2";
         }
      }

      state.Colorize(tokens, Color.Whitespace, Color.Format);

      builder.Add(new StringSymbol(input));

      return unit;
   }
}