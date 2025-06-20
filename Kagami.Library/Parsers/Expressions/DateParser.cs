﻿using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.Objects;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class DateParser : SymbolParser
{
   public DateParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex("""^(\s*)(d['"])([^'"]*)(['"])""")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var text = tokens[3].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Date, Color.Date, Color.Date);

      var _dateTime = text.Result().DateTime();
      if (_dateTime is (true, var dateTime))
      {
         builder.Add(new DateSymbol(dateTime));
         return unit;
      }
      else
      {
         return _dateTime.Exception;
      }
   }
}