﻿using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class IfAsAndParser : EndingInExpressionParser
{
   public IfAsAndParser(ExpressionBuilder builder) : base(builder, ExpressionFlags.OmitSendMessageAssign)
   {
   }

   //public override string Pattern => "^ /(/s*) /'if' /(/s+)";

   [GeneratedRegex(@"^(\s*)(if)(\s+)")]
   public override partial Regex Regex();

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace);
      return unit;
   }

   public override Optional<Unit> Suffix(ParseState state, Expression expression)
   {
      builder.Add(new AndSymbol(expression));
      return unit;
   }
}