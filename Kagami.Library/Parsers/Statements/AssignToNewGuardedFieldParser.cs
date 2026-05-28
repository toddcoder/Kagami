using Core.Monads;
using Core.Strings;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using Kagami.Library.Parsers.Expressions;
using System.Text.RegularExpressions;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class AssignToNewGuardedFieldParser : StatementParser
{
   protected const string REGEX_EQUAL = @"^(\s*)(=)(?![=>])";

   protected bool isHidden;
   protected bool mutable;
   protected string fieldName = "";
   protected Maybe<TypeConstraint> _typeConstraint = nil;

   [GeneratedRegex($@"^(\s*){REGEX_HIDDEN}{REGEX_OVERRIDE}(guard)(\s+)(let|var)(\s+)({REGEX_FIELD})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.BeginTransaction();

      isHidden = tokens[2].Text.IsNotEmpty();
      var isOverride = tokens[3].Text.IsNotEmpty();
      mutable = tokens[6].Text == "var";
      fieldName = tokens[8].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Keyword, Color.Keyword, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier);
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
            from expressionValue in getExpression(state, ExpressionFlags.Standard | ExpressionFlags.OmitIf)
            from scanned2 in state.Scan(@"^(\s*)(if)\b", Color.Whitespace, Color.Keyword)
            from predicateValue in getExpression(state, ExpressionFlags.Standard)
            select (expressionValue, predicateValue);
         if (_expression is (true, var (expression, predicate)))
         {
            var parameter = new Parameter(false, false, "", fieldName, nil, nil, false, false, false);
            var parameters = new Parameters(parameter);
            var lambdaSymbol = new LambdaSymbol(parameters, predicate);
            var assignToNewField = new AssignToNewGuardedField(mutable, fieldName, expression, _typeConstraint, isHidden, isOverride, lambdaSymbol);
            state.AddStatement(assignToNewField);
            state.CommitTransaction();

            return unit;
         }
         else
         {
            return _expression.Exception;
         }
      }
      else
      {
         state.RollBackTransaction();
         return nil;
      }
   }
}