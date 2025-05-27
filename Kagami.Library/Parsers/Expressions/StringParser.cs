using System.Text;
using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.Numbers;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class StringParser : SymbolParser
{
   public StringParser(ExpressionBuilder builder) : base(builder)
   {
   }

   //public override string Pattern => "^ /(/s*) /['mb`']? /(['\"'])";

   [GeneratedRegex(@"^(\s*)([mb`])?([""])")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var prefix = tokens[2].Text;
      var mutable = prefix == "m";
      var binary = prefix == "b";
      var symbol = prefix == "`";

      state.Colorize(tokens, Color.Whitespace, Color.StringPart, Color.String);

      var text = new StringBuilder();
      var escaped = false;
      var index = state.Index;
      var length = 0;
      var hex = false;
      var hexText = new StringBuilder();

      while (state.More)
      {
         var ch = state.CurrentSource[0];
         switch (ch)
         {
            case '"':
               if (escaped)
               {
                  text.Append('"');
                  escaped = false;
                  break;
               }

               if (hex)
               {
                  var _matchedChar = fromHex(hexText.ToString());
                  if (_matchedChar is (true, var matchedChar))
                  {
                     text.Append(matchedChar);
                  }
                  else if (_matchedChar.Exception is (true, var exception))
                  {
                     return exception;
                  }
                  else
                  {
                     return badHex(hexText.ToString());
                  }
               }

               state.Move(1);
               state.AddToken(index, length + 1, Color.String);
               if (mutable)
               {
                  builder.Add(new MutStringSymbol(text.ToString()));
               }
               else if (binary)
               {
                  builder.Add(new ByteArraySymbol(text.ToString()));
               }
               else if (symbol)
               {
                  builder.Add(new SymbolSymbol(text.ToString()));
               }
               else
               {
                  builder.Add(new StringSymbol(text.ToString()));
               }

               return unit;
            case '\\':
               if (escaped)
               {
                  text.Append('\\');
                  escaped = false;
                  break;
               }

               escaped = true;
               break;
            case 'n':
               if (escaped)
               {
                  text.Append('\n');
                  escaped = false;
                  break;
               }

               text.Append('n');
               break;
            case 'r':
               if (escaped)
               {
                  text.Append('\r');
                  escaped = false;
                  break;
               }

               text.Append('r');
               break;
            case 't':
               if (escaped)
               {
                  text.Append('\t');
                  escaped = false;
                  break;
               }

               text.Append('t');
               break;
            case 'u':
               if (escaped)
               {
                  hex = true;
                  hexText.Clear();
                  escaped = false;
                  break;
               }

               text.Append('u');
               break;
            case '{':
               if (escaped)
               {
                  hex = true;
                  hexText.Clear();
                  escaped = false;
                  break;
               }

               text.Append('{');
               break;
            default:
               if (hex)
               {
                  if (ch.Between('0').And('9') || ch.Between('a').And('f') && hexText.Length < 6)
                  {
                     hexText.Append(ch);
                  }
                  else
                  {
                     hex = false;
                     var _matchedChar = fromHex(hexText.ToString());
                     if (_matchedChar is (true, var matchedChar))
                     {
                        text.Append(matchedChar);
                     }
                     else if (_matchedChar.Exception is (true, var exception))
                     {
                        return exception;
                     }

                     if (ch == 96)
                     {
                        text.Append(ch);
                     }
                  }
               }
               else
               {
                  text.Append(ch);
               }

               escaped = false;
               break;
         }

         length++;
         state.Move(1);
      }

      return openString();
   }
}