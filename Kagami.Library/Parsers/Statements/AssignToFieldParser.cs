using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class AssignToFieldParser : EndingInExpressionParser
   {
      string fieldName;
      string operationSource;

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
         if (matchOperator(operationSource).ValueOrCast<Unit>(out var operation, out var asUnit) || asUnit.IsNotMatched)
         {
            var assignmentOperation = maybe(asUnit.IsMatched, () => operation);
            state.AddStatement(new AssignToField(fieldName, assignmentOperation, expression));

            return Unit.Matched();
         }
         else
         {
            return asUnit;
         }
      }
   }
}