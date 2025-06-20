﻿using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class ComprehensionBodyParser : SymbolParser
{
   protected List<(Symbol, Expression, PossibleExpression, string)> comprehensions;

   public ComprehensionBodyParser(ExpressionBuilder builder, List<(Symbol, Expression, PossibleExpression, string)> comprehensions) :
      base(builder)
   {
      this.comprehensions = comprehensions;
   }

   [GeneratedRegex(@"^(\s+)(for)(?![\^>])\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);
      var _innerComprehension = getComprehensionBody(state);
      if (_innerComprehension is (true, var (comparisand, source, possibleExpression)))
      {
         var image = $"for {comparisand} in {source}";
         comprehensions.Add((comparisand, source, possibleExpression, image));

         return unit;
      }
      else
      {
         return _innerComprehension.Exception;
      }
   }
}