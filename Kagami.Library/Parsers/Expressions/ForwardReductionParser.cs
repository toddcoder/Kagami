using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class ForwardReductionParser : SymbolParser
{
   public ForwardReductionParser(ExpressionBuilder builder) : base(builder)
   {
   }

    [GeneratedRegex(@$"^(\[)({REGEX_OPERATORS})(\])")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var operatorSource = tokens[2].Text;
      state.Colorize(tokens, Color.Operator, Color.Operator, Color.Operator);

      var _expression = getExpression(state, builder.Flags);
      if (_expression is (true, var expression))
      {
         builder.Add(new ForwardReductionSymbol(operatorSource, expression));
         return unit;
      }
      else
      {
         return _expression.Exception;
      }
   }
}