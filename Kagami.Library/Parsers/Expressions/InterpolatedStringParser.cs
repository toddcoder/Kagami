﻿using System.Collections.Generic;
using System.Text;
using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using Standard.Types.Numbers;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class InterpolatedStringParser : SymbolParser
   {
	   public InterpolatedStringParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'$' /[dquote]";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.StringPart, Color.String);

         var firstString = none<string>();
         var expressions = new List<Expression>();
         var suffixes = new List<string>();
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
               {
                  if (escaped)
                  {
                     text.Append('"');
                     escaped = false;
                     break;
                  }

                  if (hex)
                     if (fromHex(hexText.ToString()).If(out var machedChar, out var isNotMatched, out var exception))
                        text.Append(machedChar);
                     else if (isNotMatched)
                        return failedMatch<Unit>(badHex(hexText.ToString()));
                     else
                        return failedMatch<Unit>(exception);

                  state.Move(1);
                  state.AddToken(index, length + 1, Color.String);

                  var symbol = firstString.FlatMap(prefix =>
                  {
                     suffixes.Add(text.ToString());
                     var expressionsArray = expressions.ToArray();
                     var suffixesArray = suffixes.ToArray();
                     return (Symbol)new InterpolatedStringSymbol(prefix, expressionsArray, suffixesArray);
                  }, () => new StringSymbol(text.ToString()));
                  builder.Add(symbol);

                  return Unit.Matched();
               }
               case '(':
               {
                  if (escaped)
                  {
                     text.Append('(');
                     escaped = false;
                     break;
                  }

                  state.Move(1);
                  state.AddToken(index, length, Color.String);
                  state.AddToken(index + length, 1, Color.Structure);

                  if (firstString.IsNone)
                     firstString = text.ToString().Some();
                  else
                     suffixes.Add(text.ToString());
                  text.Clear();

                  if (getExpression(state, "^ /')'", builder.Flags, Color.Structure)
                     .If(out var expression, out var isNotMatched, out var exception))
                  {
                     expressions.Add(expression);
                     index = state.Index;
                     length = 0;
                     continue;
                  }

                  if (isNotMatched)
                     return failedMatch<Unit>(expectedExpression());

                  return failedMatch<Unit>(exception);
               }
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
               default:
               {
                  if (escaped)
                     if (ch.Between('0').And('9') || ch.Between('a').And('f') && hexText.Length < 6)
                        hexText.Append(ch);
                     else
                     {
                        escaped = false;
                        if (fromHex(hexText.ToString()).If(out var charMatched, out var isNotMatched, out var exception))
                        {
                           hexText.Append(charMatched);
                           hexText.Append(ch);
                        }
                        else if (isNotMatched)
                           return failedMatch<Unit>(badHex(hexText.ToString()));
                        else
                           return failedMatch<Unit>(exception);
                     }
                  else
                     text.Append(ch);

                  escaped = false;
               }
                  break;
            }

            length++;
            state.Move(1);
         }

         return failedMatch<Unit>(openString());
      }
   }
}