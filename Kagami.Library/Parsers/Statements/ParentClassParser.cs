using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public class ParentClassParser : StatementParser
{
   public override string Pattern => $"^ /'inherits' /(|s+|) /({REGEX_CLASS}) " + "/['({']?";

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var parentClassName = tokens[3].Text;
      var hasArguments = tokens[4].Length > 0;
      var initialize = tokens[4].Text == "{";
      state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class, Color.Structure);

      if (hasArguments)
      {
         var _expressions = getArguments(state, ExpressionFlags.Standard);
         if (_expressions is (true, var expressions))
         {
            Parent = (parentClassName, initialize, expressions);
            return unit;
         }
         else if (_expressions.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            Parent = (parentClassName, initialize, []);
            return unit;
         }
      }
      else
      {
         Parent = (parentClassName, initialize, []);
         return unit;
      }
   }

   public (string, bool, Expression[]) Parent { get; set; }
}