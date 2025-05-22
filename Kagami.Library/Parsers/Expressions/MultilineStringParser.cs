using System.Text;
using Core.Matching;
using Core.Monads;
using Core.Numbers;
using Kagami.Library.Nodes.Symbols;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class MultilineStringParser : SymbolParser
{
   public MultilineStringParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override string Pattern => "^ /(/s*) /([dquote]3) /(/r /n | /r | /n)";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.String, Color.String);
      var text = new StringBuilder();
      var escaped = false;
      var index = state.Index;
      var length = 0;
      var hex = false;
      var hexText = new StringBuilder();

      while (state.More && !state.CurrentSource.IsMatch("^ (/r /n | /r | /n) [dquote]3"))
      {
         var ch = state.CurrentSource[0];
         switch (ch)
         {
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
                     var _fromHex1 = fromHex(hexText.ToString());
                     if (_fromHex1 is (true, var fromHex1))
                     {
                        text.Append(fromHex1);
                     }
                     else if (_fromHex1.Exception)
                     {
                        return _fromHex1.Exception;
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

      var _scan = state.Scan("^ /(/r /n | /r | /n) /([dquote]3)", Color.Whitespace, Color.String);
      if (_scan)
      {
         if (hex)
         {
            var _fromHex2 = fromHex(hexText.ToString());
            if (_fromHex2 is (true, var fromHex2))
            {
               text.Append(fromHex2);
            }
            else if (_fromHex2.Exception is (true, var exception))
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
         builder.Add(new StringSymbol(text.ToString()));

         return unit;
      }
      else if (_scan.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return openString();
      }
   }
}