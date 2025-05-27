using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class ConditionalWhileParser : StatementParser
{
   //public override string Pattern => "^ /'while' /(/s+)";

   [GeneratedRegex(@"^(while)(\s+)")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.BeginTransaction();

      state.Colorize(tokens, Color.Keyword, Color.Whitespace);

      var _result =
         from comparisandValue in getExpression(state, ExpressionFlags.Comparisand)
         from scanned in state.Scan("^ /(/s*) /':='", Color.Whitespace, Color.Structure)
         from expressionValue in getExpression(state, ExpressionFlags.Standard)
         from blockValue in getBlock(state)
         select (comparisandValue, expressionValue, blockValue);

      if (_result is (true, var (comparisand, expression, block)))
      {
         state.CommitTransaction();
         state.AddStatement(new ConditionalWhile(comparisand, expression, block));

         return unit;
      }
      else
      {
         state.RollBackTransaction();
         return _result.Exception;
      }
   }
}