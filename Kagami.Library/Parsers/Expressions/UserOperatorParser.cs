using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class UserOperatorParser : SymbolParser
{
   protected Arity arity;

   public UserOperatorParser(ExpressionBuilder builder, Arity arity) : base(builder)
   {
      this.arity = arity;
   }

   [GeneratedRegex(@$"^(\s*)({REGEX_FUNCTION_NAME})")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var operatorName = tokens[2].Text;
      if (Module.Global.Value.GetOperator(operatorName, arity) is (true, var operatorType))
      {
         state.Colorize(tokens, Color.Whitespace, Color.Operator);
         builder.Add(new OperatorSymbol(operatorType));
         return unit;
      }
      else
      {
         return nil;
      }
   }
}