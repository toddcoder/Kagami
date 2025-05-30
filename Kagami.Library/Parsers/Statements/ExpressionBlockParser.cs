﻿using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public abstract class ExpressionBlockParser : StatementParser
{
   public abstract Optional<Unit> Prefix(ParseState state, Token[] tokens);

   public abstract Optional<Unit> Suffix(ParseState state, Expression expression, Block block);

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      return
         from prefix in Prefix(state, tokens)
         from expression in getExpression(state, ExpressionFlags.Standard)
         from block in getBlock(state)
         from suffix in Suffix(state, expression, block)
         select suffix;
   }
}