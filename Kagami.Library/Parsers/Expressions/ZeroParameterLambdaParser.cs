﻿using Kagami.Library.Invokables;
using Core.Monads;

namespace Kagami.Library.Parsers.Expressions
{
   public class ZeroParameterLambdaParser : LambdaParser
   {
      public override string Pattern => "^ (> (|s|) ('->' | '=>' [/r/n]+))";

      public ZeroParameterLambdaParser(ExpressionBuilder builder) : base(builder) { }

      public override IMatched<Parameters> ParseParameters(ParseState state, Token[] tokens) => new Parameters(0).Matched();
   }
}