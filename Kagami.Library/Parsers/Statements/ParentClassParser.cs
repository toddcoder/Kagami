using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class ParentClassParser : StatementParser
   {
      public override string Pattern => $"^ /'inherits' /(|s+|) /({REGEX_CLASS}) /'('?";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         var parentClassName = tokens[3].Text;
         var hasArguments = tokens[4].Text == "(";
         state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class, Color.Structure);

         if (hasArguments)
         {
            if (getArguments(state, ExpressionFlags.Standard).If(out var expressions, out var original))
            {
               Parent = (parentClassName, null, expressions);
               return Unit.Matched();
            }
            else if (original.IsNotMatched)
            {
               Parent = (parentClassName, null, new Expression[0]);
               return Unit.Matched();
            }
            else
               return original.ExceptionAs<Unit>();
         }
         else
         {
            Parent = (parentClassName, null, new Expression[0]);
            return Unit.Matched();
         }
      }

      public (string, string, Expression[]) Parent { get; set; }
   }
}