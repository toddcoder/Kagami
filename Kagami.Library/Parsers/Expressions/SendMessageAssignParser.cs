using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Kagami.Library.Operations;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class SendMessageAssignParser : EndingInExpressionParser
{
   protected string messageName = "";
   protected string operationSource = "";

   public SendMessageAssignParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override string Pattern => $"^ /(|s|) /'.' /({REGEX_FUNCTION_NAME}) /(|s|) /({REGEX_ASSIGN_OPS})? /'=' -(> ['=>'])";

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      messageName = tokens[3].Text;
      operationSource = tokens[5].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Structure, Color.Message, Color.Whitespace, Color.Operator, Color.Structure);

      return unit;
   }

   public override Optional<Unit> Suffix(ParseState state, Expression expression)
   {
      var _operation = matchOperator(operationSource);
      if (_operation is (true, var operation))
      {
         builder.Add(new SendMessageSymbol(messageName.set(), operation, expression));
         return unit;
      }
      else if (_operation.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         Maybe<Operation> _nilOperation = nil;
         builder.Add(new SendMessageSymbol(messageName.set(), _nilOperation, expression));

         return unit;
      }
   }
}