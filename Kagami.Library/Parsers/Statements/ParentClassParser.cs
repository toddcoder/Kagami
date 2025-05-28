using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class ParentClassParser : StatementParser
{
   //public override string Pattern => $"^ /(/s+) /'inherits' /(/s+) /({REGEX_CLASS}) " + "/['([']?";

   [GeneratedRegex($@"^(\s*)(:)(\s*)({REGEX_CLASS})([\(\[])?")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var parentClassName = tokens[4].Text;
      var hasArguments = tokens[5].Length > 0;
      var initialize = tokens[5].Text == "[";
      state.Colorize(tokens, Color.Whitespace, Color.Structure, Color.Whitespace, Color.Class, Color.OpenParenthesis);

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