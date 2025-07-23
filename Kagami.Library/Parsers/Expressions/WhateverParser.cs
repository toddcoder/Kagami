using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class WhateverParser : SymbolParser
{
   public WhateverParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(@)(?!\w)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      if (builder.Flags[ExpressionFlags.InLambda] || builder.Flags[ExpressionFlags.InArgument])
      {
         state.Colorize(tokens, Color.Whitespace, Color.Keyword);
         builder.Add(new AnySymbol());
      }
      else
      {
         state.Colorize(tokens, Color.Whitespace, Color.Identifier);
         builder.Add(new WhateverSymbol());
      }

      return unit;
   }
}