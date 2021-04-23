using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class SendMessageAssignParser : EndingInExpressionParser
   {
      protected string messageName;
      protected string operationSource;

      public SendMessageAssignParser(ExpressionBuilder builder) : base(builder)
      {
      }

      public override string Pattern => $"^ /(|s|) /'.' /({REGEX_FUNCTION_NAME}) /(|s|) /({REGEX_ASSIGN_OPS})? /'=' -(> ['=>'])";

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         messageName = tokens[3].Text;
         operationSource = tokens[5].Text;
         state.Colorize(tokens, Color.Whitespace, Color.Structure, Color.Message, Color.Whitespace, Color.Operator, Color.Structure);

         return Unit.Matched();
      }

      public override IMatched<Unit> Suffix(ParseState state, Expression expression)
      {
         if (matchOperator(operationSource).ValueOrCast<Unit>(out var operation, out var asUnit) || asUnit.IsNotMatched)
         {
            var assignmentOperation = maybe(asUnit.IsMatched, () => operation);
            builder.Add(new SendMessageSymbol(messageName.set(), assignmentOperation, expression));

            return Unit.Matched();
         }
         else
         {
            return asUnit;
         }
      }
   }
}