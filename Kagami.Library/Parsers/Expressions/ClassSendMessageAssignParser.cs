using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class ClassSendMessageAssignParser : SymbolParser
{
   public ClassSendMessageAssignParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex($@"^(\s*)(&)({REGEX_FUNCTION_NAME})(\s*)({REGEX_ASSIGN_OPS})?(=)(?![=>])")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var messageName = tokens[3].Text;
      var operationSource = tokens[5].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Message, Color.Message, Color.Whitespace, Color.Operator, Color.Structure);

      var _expression = getExpression(state, ExpressionFlags.Standard);
      if (_expression is (true, var expression))
      {
         var _operation = matchOperator(operationSource);
         if (_operation is (true, var operation))
         {
            builder.Add(new SendClassMessageSymbol(messageName.get(), nil, operation, expression));
         }
         else if (_operation.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            Maybe<Operation> _nilOperation = nil;
            builder.Add(new SendClassMessageSymbol(messageName.get(), nil, _nilOperation, expression));
         }

         return unit;
      }
      else
      {
         return _expression.Exception;
      }
   }
}