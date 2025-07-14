using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class MaybeParser : SymbolParser
{
   public MaybeParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(maybe)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Operator);

      var flags = builder.Flags;
      flags[ExpressionFlags.OmitMaybe] = true;
      flags[ExpressionFlags.OmitColon] = true;
      var _result =
         from booleanExpressionValue in getExpression(state, flags)
         from then in state.Scan(@"^(\s*)(:)", Color.Whitespace, Color.Operator)
         from expressionValue in getExpression(state, flags)
         select (booleanExpressionValue, expressionValue);
      if (_result is (true, var (booleanExpression, expression)))
      {
         builder.Add(new MaybeSymbol(booleanExpression, expression));
         return unit;
      }
      else
      {
         return _result.Exception;
      }
   }
}