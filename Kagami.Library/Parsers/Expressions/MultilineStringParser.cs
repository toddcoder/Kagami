using System.Text;
using Core.Monads;
using Core.Numbers;
using Core.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class MultilineStringParser : SymbolParser
   {
      public MultilineStringParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /([dquote]3) /(/r /n | /r | /n)";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
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
                        if (fromHex(hexText.ToString()).ValueOrCast<Unit>(out var matchedChar, out var asUnit))
                        {
                           text.Append(matchedChar);
                        }
                        else if (asUnit.IsFailedMatch)
                        {
                           return asUnit;
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

         if (state.Scan("^ /(/r /n | /r | /n) /([dquote]3)", Color.Whitespace, Color.String).If(out _, out var anyMatchException))
         {
            if (hex)
            {
               if (fromHex(hexText.ToString()).If(out var matchedChar, out var anyException))
               {
                  text.Append(matchedChar);
               }
               else if (anyException.If(out var exception))
               {
                  return failedMatch<Unit>(exception);
               }
               else
               {
                  return failedMatch<Unit>(badHex(hexText.ToString()));
               }
            }

            state.Move(1);
            state.AddToken(index, length + 1, Color.String);
            builder.Add(new StringSymbol(text.ToString()));

            return Unit.Matched();
         }
         else if (anyMatchException.If(out var exception))
         {
            return failedMatch<Unit>(exception);
         }
         else
         {
            return failedMatch<Unit>(openString());
         }
      }
   }
}