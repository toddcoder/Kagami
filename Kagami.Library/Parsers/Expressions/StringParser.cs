﻿using System.Text;
using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using Standard.Types.Numbers;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class StringParser : SymbolParser
   {
      public StringParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /['@b']? /(['\"'])";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var prefix = tokens[2].Text;
         var mutable = prefix == "@";
         var binary = prefix == "b";

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
                     if (fromHex(hexText.ToString()).If(out var matchedChar, out var isNotMatched, out var exception))
                        text.Append(matchedChar);
                     else if (isNotMatched)
                        return failedMatch<Unit>(badHex(hexText.ToString()));
                     else
                        return failedMatch<Unit>(exception);

                  state.Move(1);
                  state.AddToken(index, length + 1, Color.String);
                  if (mutable)
                     return "Mutable not implemented yet".FailedMatch<Unit>();
                  else if (binary)
                     return "Byte array not implemented yet".FailedMatch<Unit>();
                  else
                  {
                     builder.Add(new StringSymbol(text.ToString()));
                     return Unit.Matched();
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
                  if (hex)
                     if (ch.Between('0').And('9') || ch.Between('a').And('f') && hexText.Length < 6)
                        hexText.Append(ch);
                     else
                     {
                        hex = false;
                        if (fromHex(hexText.ToString()).If(out var matchedChar, out var original))
                           text.Append(matchedChar);
                        else if (original.IsFailedMatch)
                           return original.ExceptionAs<Unit>();

                        if (ch == 96)
                           text.Append(ch);
                     }
                  else
                     text.Append(ch);

                  escaped = false;
                  break;
            }

            length++;
            state.Move(1);
         }

         return failedMatch<Unit>(openString());
      }
   }
}