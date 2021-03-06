﻿using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class ExpressionStatementParser : StatementParser
   {
      protected bool returnExpression;
      protected IMaybe<TypeConstraint> _typeConstraint;

      public ExpressionStatementParser(bool returnExpression, IMaybe<TypeConstraint> typeConstraint)
      {
         this.returnExpression = returnExpression;
         _typeConstraint = typeConstraint;
      }

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         var flags = ExpressionFlags.Standard;
         if (returnExpression)
         {
            flags |= ExpressionFlags.OmitSendMessageAssign;
         }

         if (getExpression(state, flags).ValueOrCast<Unit>(out var expression, out var asUnit))
         {
            state.AddStatement(new ExpressionStatement(expression, returnExpression, _typeConstraint));
            return Unit.Matched();
         }
         else
         {
            return asUnit;
         }
      }

      public override bool UpdateIndexOnParseOnly => true;
   }
}