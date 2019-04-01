using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class ForExpressionParser : EndingInExpressionParser
   {
      public ForExpressionParser(ExpressionBuilder builder, ExpressionFlags flags = ExpressionFlags.Standard) : base(builder, flags) { }

      public override string Pattern => "^ /(|s|) /'%'";

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Operator);
         return Unit.Matched();
      }

      public override IMatched<Unit> Suffix(ParseState state, Expression expression)
      {
         var fieldName = newLabel("index");
         state.ForExpression = (fieldName, expression).Some();
         builder.Add(new FieldSymbol(fieldName));

         return Unit.Matched();
      }
   }
}