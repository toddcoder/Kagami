using System.Text.RegularExpressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class PartialLambdaParser : SymbolParser
{
   public PartialLambdaParser(ExpressionBuilder builder) : base(builder)
   {
   }

   //public override string Pattern => "^ /(/s*) /'('";

   [GeneratedRegex(@"^([ \t]*)(\()")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.BeginTransaction();
      state.Colorize(tokens, Color.Whitespace, Color.OpenParenthesis);
      var _lambda = getPartialLambda(state);
      if (_lambda is (true, var lambda))
      {
         builder.Add(lambda);
         state.CommitTransaction();

         return unit;
      }
      else
      {
         state.RollBackTransaction();
         return _lambda.Exception;
      }
   }
}