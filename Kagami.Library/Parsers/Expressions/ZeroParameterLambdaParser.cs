using Core.Matching;
using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Symbols;
using System.Text.RegularExpressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Expressions;

public partial class ZeroParameterLambdaParser : SymbolParser
{
   [GeneratedRegex(@"^([ \t]*)(->)")]
   public override partial Regex Regex();

   public ZeroParameterLambdaParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Lambda);
      state.BeginTransaction();
      state.CreateReturnType();

      var _result =
         from typeConstraint in parseTypeConstraint(state)
         from block in getLambdaBlock(!state.CurrentSource.IsMatch("^ /s* '{' -(>'}')"), false, state, builder.Flags & ~ExpressionFlags.Comparisand | ExpressionFlags.InLambda, typeConstraint.Maybe)
         select new LambdaSymbol(Parameters.Empty, block);
      if (_result is (true, var lambdaSymbol))
      {
         builder.Add(lambdaSymbol);
         state.RemoveReturnType();
         state.CommitTransaction();

         return unit;
      }
      else
      {
         state.RollBackTransaction();
         state.RemoveReturnType();

         return _result.Exception;
      }
   }
}