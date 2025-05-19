using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class ImplicitSymbolParser : EndingInValueParser
{
   public ImplicitSymbolParser(ExpressionBuilder builder) : base(builder) { }

   public override string Pattern => "^ /(|s|) /'^' -(> /s)";

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Structure);
      return unit;
   }

   public override Optional<Unit> Suffix(ParseState state, Symbol value)
   {
      var implicitExpressionState = state.ImplicitExpressionState;
      if (implicitExpressionState.Symbol1.IsNone)
      {
         implicitExpressionState.Symbol1 = value.Some();
         builder.Add(new FieldSymbol(implicitExpressionState.FieldName1));

         return unit;
      }
      else if (implicitExpressionState.Symbol2.IsNone)
      {
         implicitExpressionState.Symbol2 = value.Some();
         builder.Add(new FieldSymbol(implicitExpressionState.FieldName2));

         return unit;
      }
      else
      {
         return nil;
      }
   }
}