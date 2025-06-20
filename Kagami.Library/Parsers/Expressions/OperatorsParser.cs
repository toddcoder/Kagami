﻿using System.Text.RegularExpressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class OperatorsParser : SymbolParser
{
   public OperatorsParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex($@"^(\s*)({REGEX_OPERATORS}{{1,2}})(?!{REGEX_OPERATORS})(\s*)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.BeginTransaction();

      var source = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Whitespace);

      var _symbol = getOperator(state, source, builder.Flags);
      if (_symbol is (true, var symbol))
      {
         builder.Add(symbol);
         state.CommitTransaction();

         return unit;
      }
      else
      {
         state.RollBackTransaction();
         return _symbol.Exception;
      }
   }
}