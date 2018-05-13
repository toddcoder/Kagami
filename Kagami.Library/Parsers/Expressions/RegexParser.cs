using System.Text;
using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;

namespace Kagami.Library.Parsers.Expressions
{
   public class RegexParser : SymbolParser
   {
      public RegexParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => @"^ /(|s|) /'\'";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Structure);

         var pattern = new StringBuilder();
         var type = RegexParsingType.Outside;
         var ignoreCase = false;
         var multiline = false;
         var global = false;

         while (state.More)
         {
            var ch = state.CurrentSource[0];

            switch (type)
            {
               case RegexParsingType.Outside:
                  switch (ch)
                  {
                     case '\\':
                        state.AddToken(Color.Structure);
                        state.Move(1);
                        builder.Add(new RegexSymbol(pattern.ToString(), ignoreCase, multiline, global));

                        return Unit.Matched();
                     case '\'':
                        type = RegexParsingType.WaitingForSingleQuote;
                        pattern.Append(ch);
                        state.AddToken(Color.String);
                        break;
                     case '"':
                        type = RegexParsingType.WaitingForDoubleQuote;
                        pattern.Append(ch);
                        state.AddToken(Color.String);
                        break;
                     case ';':
                        type = RegexParsingType.AwaitingOption;
                        state.AddToken(Color.Structure);
                        break;
                     default:
                        pattern.Append(ch);
                        var color = Color.Operator;
                        switch (ch)
                        {
                           case '{':
                           case '}':
                           case '>':
                           case '<':
                           case '[':
                           case ']':
                           case ',':
                           case '|':
                           case '(':
                           case ')':
                              color = Color.Structure;
                              break;
                           default:
                              if (char.IsNumber(ch))
                                 color = Color.Number;
                              else if (char.IsLetter(ch))
                                 color = Color.Identifier;
                              break;
                        }

                        state.AddToken(color);
                        break;
                  }

                  break;
               case RegexParsingType.WaitingForSingleQuote:
                  switch (ch)
                  {
                     case '\'':
                        type = RegexParsingType.Outside;
                        break;
                     case '\\':
                        type = RegexParsingType.EscapedSingleQuote;
                        break;
                  }

                  pattern.Append(ch);
                  state.AddToken(Color.String);
                  break;
               case RegexParsingType.WaitingForDoubleQuote:
                  switch (ch)
                  {
                     case '"':
                        type = RegexParsingType.Outside;
                        break;
                     case '\\':
                        type = RegexParsingType.EscapedDoubleQuote;
                        break;
                  }

                  pattern.Append(ch);
                  state.AddToken(Color.String);
                  break;
               case RegexParsingType.EscapedSingleQuote:
                  type = RegexParsingType.WaitingForSingleQuote;
                  pattern.Append(ch);
                  state.AddToken(Color.String);
                  break;
               case RegexParsingType.EscapedDoubleQuote:
                  type = RegexParsingType.WaitingForDoubleQuote;
                  pattern.Append(ch);
                  state.AddToken(Color.String);
                  break;
               case RegexParsingType.AwaitingOption:
                  switch (ch)
                  {
                     case 'i':
                     case 'I':
                        ignoreCase = true;
                        break;
                     case 'm':
                     case 'M':
                        multiline = true;
                        break;
                     case 'g':
                     case 'G':
                        global = true;
                        break;
                     case '\\':
                        state.AddToken(Color.Structure);
                        state.Move(1);
                        builder.Add(new RegexSymbol(pattern.ToString(), ignoreCase, multiline, global));
                        return Unit.Matched();
                     default:
                        return $"Didn't understand option '{ch}'".FailedMatch<Unit>();
                  }

                  state.AddToken(Color.Symbol);
                  break;
            }

            state.Move(1);
         }

         return "Open regex".FailedMatch<Unit>();
      }
   }
}