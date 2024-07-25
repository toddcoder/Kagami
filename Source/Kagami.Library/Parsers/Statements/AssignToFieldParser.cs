using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class AssignToFieldParser : EndingInExpressionParser
   {
      protected string fieldName;
      protected string operationSource;

      public override string Pattern => $"^ /({REGEX_FIELD}) /(/s*) /({REGEX_ASSIGN_OPS})? /'=' -(> ['=>'])";

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         fieldName = tokens[1].Text;
         operationSource = tokens[3].Text;
         state.Colorize(tokens, Color.Identifier, Color.Whitespace, Color.Operator, Color.Structure);

         return Unit.Matched();
      }

      public override IMatched<Unit> Suffix(ParseState state, Expression expression)
      {
         var assignmentOperation = none<Operation>();

         if (matchOperator(operationSource).If(out var operation, out var _exception))
         {
            assignmentOperation = operation.Some();
         }
         else if (_exception.If(out var exception))
         {
            return failedMatch<Unit>(exception);
         }

         state.AddStatement(new AssignToField(fieldName, assignmentOperation, expression));
         return Unit.Matched();
      }
   }
}