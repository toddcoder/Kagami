using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class SubexpressionParser : SymbolParser
{
   public SubexpressionParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(\()")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.BeginTransaction();
      state.Colorize(tokens, Color.Whitespace, Color.OpenParenthesis);
      var openIndex = state.LastTokenIndex;
      Maybe<int> _closeIndex = nil;

      var flags = builder.Flags;
      builder.Flags[ExpressionFlags.OmitComma] = false;
      var _expression = getExpression(state, @"(\))", flags, (_, i) =>
      {
         switch (i)
         {
            case 1:
               _closeIndex = state.LastTokenIndex + 1;
               return Color.CloseParenthesis;
            default:
               return Color.Whitespace;
         }
      });
      if (_expression is (true, var expression))
      {
         if (expression.Symbols.Any(s => s is CommaSymbol) && _closeIndex is (true, var closeIndex))
         {
            state[openIndex] = Color.Collection;
            state[closeIndex] = Color.Collection;
         }

         builder.Add(new SubexpressionSymbol(expression));
         state.CommitTransaction();

         builder.Flags = flags;

         return unit;
      }
      else if (_expression.Exception)
      {
         state.RollBackTransaction();
         state.BeginTransaction();
         var _lambdaSymbol = getPartialLambda(state);
         if (_lambdaSymbol is (true, var lambdaSymbol))
         {
            state.CommitTransaction();
            builder.Add(lambdaSymbol);

            return unit;
         }
         else if (_lambdaSymbol.Exception is (true, var exception))
         {
            state.RollBackTransaction();
            return exception;
         }
         else
         {
            state.RollBackTransaction();
            return nil;
         }
      }
      else
      {
         return nil;
      }
   }
}