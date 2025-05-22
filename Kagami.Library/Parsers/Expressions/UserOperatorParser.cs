using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class UserOperatorParser : SymbolParser
{
   public UserOperatorParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override string Pattern => $"^ /(/s*) /({REGEX_FUNCTION_NAME}) /(/s+)";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var operatorName = tokens[2].Text;
      if (Module.Global.Value.OperatorExists(operatorName))
      {
         state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Whitespace);
         builder.Add(new OperatorSymbol(operatorName));
         return unit;
      }
      else
      {
         return nil;
      }
   }
}