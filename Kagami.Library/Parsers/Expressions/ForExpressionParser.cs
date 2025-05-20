using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class ForExpressionParser : EndingInExpressionParser
{
   public ForExpressionParser(ExpressionBuilder builder, ExpressionFlags flags = ExpressionFlags.Standard) :
      base(builder, flags) { }

   public override string Pattern => "^ /(|s|) /'%' -(> '>')";

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Structure);
      return unit;
   }

   public override Optional<Unit> Suffix(ParseState state, Expression expression)
   {
      var implicitExpressionState = state.ImplicitExpressionState;
      if (!implicitExpressionState.Symbol1)
      {
         implicitExpressionState.Symbol1 = expression;
         builder.Add(new FieldSymbol(implicitExpressionState.FieldName1));

         return unit;
      }
      else if (!implicitExpressionState.Symbol2)
      {
         implicitExpressionState.Symbol2 = expression;
         builder.Add(new FieldSymbol(implicitExpressionState.FieldName2));

         return unit;
      }
      else
      {
         return nil;
      }
   }
}