using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class AssignFromLoopParser : StatementParser
{
   protected static Optional<(Expression condition, Expression expression)> getReturn(ParseState state)
   {
      var returnFromLoopParser = new ReturnFromLoopParser();
      return returnFromLoopParser.Scan(state).Map(_ => (returnFromLoopParser.Condition, returnFromLoopParser.Expression));
   }

   //public override string Pattern => $"^ (/('var' | 'let') /(/s+))? /({REGEX_FIELD}) /b";

   [GeneratedRegex($@"^(?:(var|let)(\s+))?({REGEX_FIELD})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.BeginTransaction();

      var isNew = tokens[1].Text.IsNotEmpty();
      var mutable = tokens[1].Text == "var";
      var fieldName = tokens[3].Text;
      state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Identifier);

      var _result =
         from typeConstraintValue in parseTypeConstraint(state)
         from scanned in state.Scan($@"^(\s*)(=)(\s*)(loop)({REGEX_EOL})", Color.Whitespace, Color.Structure,
            Color.Whitespace, Color.Keyword, Color.Whitespace)
         from blockValue in getBlock(state)
         from pair in getReturn(state)
         select (typeConstraintValue, blockValue, pair);

      if (_result is (true, var (typeConstraint, block, (condition, expression))))
      {
         state.AddStatement(new AssignFromLoop(isNew, mutable, fieldName, typeConstraint.Maybe, block, condition, expression));
         state.CommitTransaction();

         return unit;
      }
      else
      {
         state.RollBackTransaction();
         return nil;
      }
   }
}