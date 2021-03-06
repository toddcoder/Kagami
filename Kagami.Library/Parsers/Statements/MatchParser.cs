﻿using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.Nodes.NodeFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class MatchParser : StatementParser
   {
      public override string Pattern => $"^ (/('var' | 'let') /(|s|) /({REGEX_FIELD}) /(|s|) /'=' /(|s|))? /'match' /(/s+)";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         var mutable = tokens[1].Text == "var";
         var fieldName = tokens[3].Text;
         var assignment = fieldName.IsNotEmpty();

         state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Structure, Color.Whitespace,
            Color.Keyword, Color.Whitespace);

         if (getExpression(state, ExpressionFlags.Standard).ValueOrCast<Unit>(out var expression, out var asUnit))
         {
            var matchField = newLabel("match");
            state.AddStatement(new AssignToNewField(false, matchField, expression));

            state.Advance();
            var caseParser = new CaseParser(fieldName, mutable, assignment, matchField, true, CaseType.Statement);
            if (caseParser.Scan(state).ValueOrOriginal(out _, out asUnit))
            {
               var ifStatement = caseParser.If;
               addMatchElse(ifStatement);
               state.AddStatement(ifStatement);
               state.Regress();

               return Unit.Matched();
            }
            else
            {
               state.Regress();
               return asUnit;
            }
         }
         else
         {
            return asUnit;
         }
      }
   }
}