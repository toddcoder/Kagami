using System;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.Numbers;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public abstract class ExpressionInMiddleParser : SymbolParser
   {
      protected string pattern;
      protected Color[] colors;
      protected Bits32<ExpressionFlags> flags;


      public ExpressionInMiddleParser(ExpressionBuilder builder, string pattern, params Color[] colors) : base(builder)
      {
         this.pattern = pattern;
         this.colors = colors;
         flags = builder.Flags;
      }

      public abstract IMatched<Unit> Prefix(ParseState state, Token[] tokens);

      public abstract IMatched<Unit> Suffix(ParseState state, Expression expression);

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var result = Prefix(state, tokens).Map(u => getExpression(state, pattern, flags, colors)).Map(e => Suffix(state, e));
         if (result.IsFailedMatch)
            return OnFailure(state, result.Exception);
         else
            return result;
      }

      public virtual IMatched<Unit> OnFailure(ParseState state, Exception exception) => failedMatch<Unit>(exception);
   }
}