﻿using System.Text.RegularExpressions;
using Kagami.Library.Invokables;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class OneParameterLambdaParser : LambdaParser
{
   //public override string Pattern => $"^ /(/s*) /({REGEX_FIELD}) /b (> /s* ('->' | '=>' [/r/n]+))";

   [GeneratedRegex($@"^(\s*)({REGEX_FIELD})\b(?=\s*(?:->|=>[\r\n]+))")]
   public override partial Regex Regex();

   public OneParameterLambdaParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override Optional<Parameters> ParseParameters(ParseState state, Token[] tokens)
   {
      var name = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Identifier);

      return new Parameters(new Parameter(false, "", name, nil, nil, false, false));
   }
}