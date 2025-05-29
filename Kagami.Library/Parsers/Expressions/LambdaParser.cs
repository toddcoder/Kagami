using Core.Matching;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public abstract class LambdaParser : SymbolParser
{
   protected LambdaParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public abstract Optional<Parameters> ParseParameters(ParseState state, Token[] tokens);

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.BeginTransaction();
      state.CreateReturnType();

      var _result =
         from parameters in ParseParameters(state, tokens)
         from scanned in state.Scan(@"^(\s*)(->)", Color.Whitespace, Color.Structure)
         from typeConstraint in parseTypeConstraint(state)
         from block in getLambdaBlock(!state.CurrentSource.IsMatch("^ /s* '{'"), state,
            builder.Flags & ~ExpressionFlags.Comparisand | ExpressionFlags.InLambda, typeConstraint.Maybe)
         select new LambdaSymbol(parameters, block);
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