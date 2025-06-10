using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class InlineIfParser : SymbolParser
{
   public InlineIfParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s+)(\?)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Operator);

      var _result =
         from ifTrueValue in getExpression(state, builder.Flags | ExpressionFlags.OmitColon)
         from scanned in state.Scan(@"^(\s*)(:)", Color.Whitespace, Color.Operator)
         from ifFalseValue in getExpression(state, builder.Flags)
         select (ifTrueValue, ifFalseValue);

      if (_result is (true, var (ifTrue, ifFalse)))
      {
         builder.Add(new InlineIfSymbol(ifTrue, ifFalse));

         return unit;
      }
      else
      {
         return _result.Exception;
      }
   }
}