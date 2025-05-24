using System.Text;
using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class AlternateStringParser : SymbolParser
{
   public AlternateStringParser(ExpressionBuilder builder) : base(builder)
   {
   }

   //public override string Pattern => "^ /(/s*) /['rl'] /[dquote]";

   [GeneratedRegex(@"^(\s*)([rl])("")", RegexOptions.Compiled)]
   public partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var type = tokens[2].Text switch
      {
         "r" => AlternateStringType.Raw,
         "l" => AlternateStringType.List,
         _ => AlternateStringType.Standard
      };

      state.Colorize(tokens, Color.Whitespace, Color.StringPart, Color.String);

      var stringBuilder = new StringBuilder();
      var escaped = false;
      var start = state.Index;
      var length = 0;

      while (state.More)
      {
         var ch = state.CurrentSource[0];
         switch (ch)
         {
            case '"':
               if (escaped)
               {
                  stringBuilder.Append('"');
                  escaped = false;
               }
               else if (state.CurrentSource.Drop(1).StartsWith("\""))
               {
                  escaped = true;
               }
               else
               {
                  state.Move(1);
                  state.AddToken(start, length + 1, Color.String);
                  if (type == AlternateStringType.List)
                  {
                     builder.Add(new StringListSymbol(stringBuilder.ToString()));
                  }
                  else
                  {
                     builder.Add(new StringSymbol(stringBuilder.ToString()));
                  }

                  return unit;
               }

               break;
            default:
               stringBuilder.Append(ch);
               escaped = false;
               break;
         }

         length++;
         state.Move(1);
      }

      return openString();
   }
}