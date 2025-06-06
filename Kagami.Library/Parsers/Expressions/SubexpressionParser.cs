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

   [GeneratedRegex(@"^(\s*)(\()(,)?")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.BeginTransaction();
      var monoTuple = tokens[3].Text == ",";
      state.Colorize(tokens, Color.Whitespace, Color.OpenParenthesis, Color.Structure);

      var _expression = getExpression(state, @"^(\))", builder.Flags & ~ExpressionFlags.OmitComma, Color.CloseParenthesis);
      if (_expression is (true, var expression))
      {
         builder.Add(new SubexpressionSymbol(expression, monoTuple));
         state.CommitTransaction();

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