using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Kagami.Library.Nodes.NodeFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class NewObjectParser : SymbolParser
   {
      public override string Pattern => $"^ /(|s|) /({REGEX_CLASS}) /':' /({REGEX_EOL})";

      public NewObjectParser(ExpressionBuilder builder) : base(builder) { }

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var className = tokens[2].Text;
         state.Colorize(tokens, Color.Whitespace, Color.Class, Color.Structure, Color.Whitespace);

         var tempObjectName = newLabel(className);
         var outerBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
         outerBuilder.Add(new CallSysFunctionSymbol0(sys => sys.NewParameterlessObject(className, tempObjectName),
            $"newParameterlessObject({className}, {tempObjectName})"));
         outerBuilder.Add(new InvokeSymbol(className, new Expression[] { }, none<LambdaSymbol>()));

         var setPropertyParser = new SetPropertyParser(builder, tempObjectName, outerBuilder);
         while (true)
            if (setPropertyParser.Scan(state).If(out _, out var isNotMatched, out var exception)) { }
            else if (isNotMatched)
               break;
            else
               return failedMatch<Unit>(exception);

         if (outerBuilder.ToExpression().If(out var expression, out var outerException))
            builder.Add(expression);
         else
            return failedMatch<Unit>(outerException);

         return Unit.Matched();
      }
   }
}