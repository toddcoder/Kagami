using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class ReturnFromLoopParser : StatementParser
{
   //public override string Pattern => "^ /'return' /b";

   [GeneratedRegex(@"^(return)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.BeginTransaction();
      state.Colorize(tokens, Color.Keyword);

      var _result =
         from expressionValue in getExpression(state, ExpressionFlags.OmitIf)
         from @if in state.Scan(@"^(\s*)(if)\b", Color.Whitespace, Color.Keyword)
         from conditionValue in getExpression(state, ExpressionFlags.Standard)
         select (expressionValue, conditionValue);

      if (_result is (true, var (expression, condition)))
      {
         Condition = condition;
         Expression = expression;

         state.CommitTransaction();

         return unit;
      }
      else
      {
         state.RollBackTransaction();
         return _result.Exception;
      }
   }

   public Expression Condition { get; set; } = Expression.Empty;

   public Expression Expression { get; set; } = Expression.Empty;
}