using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class EmptyMemoParser : SymbolParser
{
   public EmptyMemoParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(\{:)(?!\})")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Collection);

      var _lambda = ParserFunctions.getAnyLambda(state, builder.Flags);
      if (_lambda is (true, var lambdaSymbol))
      {
         builder.Add(new EmptyMemoSymbol(lambdaSymbol));
         state.Scan(@"^(\s*)(\})", Color.Whitespace, Color.Collection);

         return unit;
      }
      else
      {
         return _lambda.Exception;
      }
   }
}