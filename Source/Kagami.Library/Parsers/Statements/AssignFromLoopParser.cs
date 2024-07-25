using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class AssignFromLoopParser : StatementParser
   {
      protected static IMatched<(Expression condition, Expression expression)> getReturn(ParseState state)
      {
         var returnFromLoopParser = new ReturnFromLoopParser();
         return returnFromLoopParser.Scan(state).Map(_ => (returnFromLoopParser.Condition, returnFromLoopParser.Expression));
      }

      public override string Pattern => $"^ (/('var' | 'let') /(|s+|))? /({REGEX_FIELD}) /b";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         state.BeginTransaction();

         var isNew = tokens[1].Text.IsNotEmpty();
         var mutable = tokens[1].Text == "var";
         var fieldName = tokens[3].Text;
         state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Identifier);

         var result =
            from typeConstraint in parseTypeConstraint(state)
            from scanned in state.Scan($"^ /(|s|) /'=' /(|s|) /'loop' /({REGEX_EOL})", Color.Whitespace, Color.Structure,
               Color.Whitespace, Color.Keyword, Color.Whitespace)
            from block in getBlock(state)
            from pair in getReturn(state)
            select (typeConstraint, block, pair);

         if (result.If(out var tuple, out _))
         {
            var (typeConstraint, block, (condition, expression)) = tuple;
            state.AddStatement(new AssignFromLoop(isNew, mutable, fieldName, typeConstraint, block, condition, expression));
            state.CommitTransaction();

            return Unit.Matched();
         }
         else
         {
            state.RollBackTransaction();
            return notMatched<Unit>();
         }
      }
   }
}