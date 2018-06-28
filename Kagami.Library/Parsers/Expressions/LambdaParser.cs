using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public abstract class LambdaParser : SymbolParser
   {
      protected LambdaParser(ExpressionBuilder builder) : base(builder) { }

      public abstract IMatched<Parameters> ParseParameters(ParseState state, Token[] tokens);

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.BeginTransaction();

         var result =
            from parameters in ParseParameters(state, tokens)
            from scanned in state.Scan("^ /(|s|) /('->' | '=>')", Color.Whitespace, Color.Structure)
            from block in getLambdaBlock(scanned.Contains("->"), state, builder.Flags)
            select new LambdaSymbol(parameters, block);
         if (result.If(out var lambdaSymbol, out var original))
         {
            builder.Add(lambdaSymbol);
            state.CommitTransaction();

            return Unit.Matched();
         }
         else
         {
            state.RollBackTransaction();
            return original.Unmatched<Unit>();
         }
      }
   }
}