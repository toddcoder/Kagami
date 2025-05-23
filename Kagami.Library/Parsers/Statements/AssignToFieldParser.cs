using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public class AssignToFieldParser : EndingInExpressionParser
{
   protected string fieldName = "";
   protected string operationSource = "";

   public override string Pattern => $"^ /(/s*) /({REGEX_FIELD}) /(/s*) /({REGEX_ASSIGN_OPS})? /'=' -(> ['=>'])";

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      fieldName = tokens[2].Text;
      operationSource = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Operator, Color.Structure);

      return unit;
   }

   public override Optional<Unit> Suffix(ParseState state, Expression expression)
   {
      Maybe<Operation> _assignmentOperation = nil;

      var _operation = matchOperator(operationSource);
      if (_operation is (true, var operation))
      {
         _assignmentOperation = operation;
      }
      else if (_operation.Exception is (true, var exception))
      {
         return exception;
      }

      state.AddStatement(new AssignToField(fieldName, _assignmentOperation, expression));
      return unit;
   }
}