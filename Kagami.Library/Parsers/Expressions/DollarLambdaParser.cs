using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Expressions;

public partial class DollarLambdaParser : SymbolParser
{
   public DollarLambdaParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"(\s*)(&)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.BeginTransaction();
      state.Colorize(tokens, Color.Whitespace, Color.Lambda);
      var _expression = getExpression(state, builder.Flags | ExpressionFlags.InLambda);
      if (_expression is (true, var expression))
      {
         if (expression.Symbols.Length > 0 && expression.Symbols[0] is LambdaSymbol lambdaSymbol)
         {
            builder.Add(lambdaSymbol);
            state.CommitTransaction();
            return unit;
         }
      }
      state.RollBackTransaction();
      return nil;
   }
}