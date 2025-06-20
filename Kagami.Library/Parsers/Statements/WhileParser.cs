﻿using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class WhileParser : StatementParser
{
   [GeneratedRegex(@"^(\s*)(while|until)(?![>\^])\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var isWhile = tokens[2].Text == "while";
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      var _result =
         from expression in getExpression(state, ExpressionFlags.Standard)
         from block in getBlock(state)
         select new While(expression, block, isWhile);
      if (_result is (true, var statement))
      {
         state.AddStatement(statement);
         return unit;
      }
      else
      {
         return _result.Exception;
      }
   }
}