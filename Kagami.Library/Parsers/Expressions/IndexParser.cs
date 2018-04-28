using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class IndexParser : SymbolParser
   {
      public IndexParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /'['";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Structure);

         return getArguments(state, builder.Flags).Map(e =>
         {
            if (state.Scan($"^ /(|s|) /({REGEX_ASSIGN_OPS})? /'='", Color.Whitespace, Color.Operator, Color.Structure)
               .If(out var opSource, out var isNotMatched, out var exception))
               if (getExpression(state, builder.Flags).If(out var expression, out var original))
               {
                  opSource = opSource.SkipWhile(" ").Take(1);
                  var operation = matchOperator(opSource)
                     .FlatMap(o => o.Some(), none<Operations.Operation>, _ => none<Operations.Operation>());
                  builder.Add(new IndexSetterSymbol(e, expression, operation));
                  return Unit.Matched();
               }
               else
                  return original.Unmatched<Unit>();
            else if (isNotMatched)
               builder.Add(new IndexSymbol(e));
            else
               return failedMatch<Unit>(exception);

            return Unit.Matched();
         });
      }
   }
}