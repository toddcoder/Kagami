using System.Text;
using System.Text.RegularExpressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class MultilineCommentParser : StatementParser
{
   [GeneratedRegex(@"^(\s*)(/\*)")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Comment);

      var buffer = new StringBuilder();
      var possibleEnd = false;
      var startIndex = state.Index;

      while (state.More)
      {
         var ch = state.CurrentSource[0];
         if (possibleEnd)
         {
            if (ch == '/')
            {
               buffer.Append(ch);
               state.Move(1);
               state.Colorize(startIndex, buffer.ToString(), Color.Comment);

               return unit;
            }
            else
            {
               possibleEnd = false;
               buffer.Append(ch);
               state.Move(1);
            }
         }
         else
         {
            if (ch == '*')
            {
               possibleEnd = true;
            }

            buffer.Append(ch);
            state.Move(1);
         }
      }

      return fail("Open comment");
   }
}