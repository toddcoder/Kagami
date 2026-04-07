using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using Core.Monads;
using Core.Strings;
using Kagami.Library.Parsers.Expressions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class AssignToNewFieldParser : StatementParser
{
   protected const string REGEX_EQUAL = @"^(\s*)(=)(?![=>])";

   protected bool isHidden;
   protected bool mutable;
   protected string fieldName = "";
   protected Maybe<TypeConstraint> _typeConstraint = nil;

   [GeneratedRegex($@"^(\s*){REGEX_HIDDEN}{REGEX_OVERRIDE}{REGEX_PARAM}(let|var)(\s+)({REGEX_FIELD})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.BeginTransaction();

      isHidden = tokens[2].Text.IsNotEmpty();
      var isOverride = tokens[3].Text.IsNotEmpty();
      var isParam = tokens[4].Text.IsNotEmpty();
      mutable = tokens[5].Text == "var";
      fieldName = tokens[7].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Keyword, Color.Keyword, Color.Keyword, Color.Whitespace, Color.Identifier);
      var _parsedTypeConstraint = parseTypeConstraint(state);
      if (_parsedTypeConstraint is (true, var parsedTypeConstraint))
      {
         _typeConstraint = parsedTypeConstraint.Maybe;
      }
      else if (_parsedTypeConstraint.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         _typeConstraint = nil;
      }

      if (state.LookAhead(REGEX_EQUAL))
      {
         var _expression =
            from scanned in state.Scan(REGEX_EQUAL, Color.Whitespace, Color.Structure)
            from expressionValue in getExpression(state, ExpressionFlags.Standard)
            select expressionValue;
         if (_expression is (true, var expression))
         {
            var assignToNewField = new AssignToNewField(mutable, fieldName, expression, _typeConstraint, isHidden, isOverride);
            if (isParam)
            {
               state.RegisterParameter(assignToNewField);
            }
            else
            {
               state.AddStatement(assignToNewField);
            }

            state.CommitTransaction();

            return unit;
         }
         else
         {
            return _expression.Exception;
         }
      }
      else if (_typeConstraint is (true, var typeConstraint))
      {
         var defineNewField = new DefineNewField(mutable, fieldName, typeConstraint, isHidden, isOverride, isParam);
         if (isParam)
         {
            state.RegisterParameter(defineNewField);
         }
         else
         {
            state.AddStatement(defineNewField);
         }

         state.CommitTransaction();
         return unit;
      }
      else
      {
         state.RollBackTransaction();
         return nil;
      }
   }
}