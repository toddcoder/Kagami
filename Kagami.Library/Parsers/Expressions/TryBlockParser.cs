using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class TryBlockParser : SymbolParser
{
   public TryBlockParser(ExpressionBuilder builder) : base(builder)
   {
   }

   //public override string Pattern => $"^ /(/s*) /'try' /({REGEX_EOL})";

   [GeneratedRegex(@"^(\s*)(try) \b", RegexOptions.Compiled)]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace);

      var _block = getBlock(state);
      if (_block is (true, var block))
      {
         block.AddReturnIf(new UnitSymbol());
         var lambda = new LambdaSymbol(0, _block);
         var invokeBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
         invokeBuilder.Add(lambda);
         invokeBuilder.Add(new PostfixInvokeSymbol([]));
         var _invokeExpression = invokeBuilder.ToExpression();
         if (_invokeExpression is (true, var invokeExpression))
         {
            builder.Add(new TrySymbol(invokeExpression));
            return unit;
         }
         else
         {
            return _invokeExpression.Exception;
         }
      }
      else
      {
         return _block.Map(_ => unit);
      }
   }
}