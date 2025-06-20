﻿using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Nodes.NodeFunctions;
using Yield = Kagami.Library.Nodes.Statements.Yield;

namespace Kagami.Library.Parsers.Statements;

public partial class YieldParser : EndingInExpressionParser
{
   protected bool all;

   [GeneratedRegex(@"^(\s*)(yield)(?:(\s+)(all))?(\s+)")]
   public override partial Regex Regex();

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      all = tokens[4].Text == "all";
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Keyword, Color.Whitespace);

      return unit;
   }

   public override Optional<Unit> Suffix(ParseState state, Expression expression)
   {
      if (all)
      {
         var placeholderName = newLabel("yieldIndex");
         var block = new Block(new Yield(new Expression(new FieldSymbol(placeholderName))));
         var @for = new For(new PlaceholderSymbol("-" + placeholderName), expression, block);
         state.AddStatement(@for);
      }
      else
      {
         state.AddStatement(new Yield(expression));
      }

      state.SetYieldFlag();
      return unit;
   }
}