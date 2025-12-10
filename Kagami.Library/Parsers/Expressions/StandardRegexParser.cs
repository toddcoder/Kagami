using System.Text;
using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class StandardRegexParser : SymbolParser
{
   public StandardRegexParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(x')")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Regex);

      var pattern = new StringBuilder();
      var main = true;
      var ignoreCase = false;
      var multiline = false;
      var global = false;
      var textOnly = false;
      var escaped = false;

      while (state.More)
      {
         var ch = state.CurrentSource[0];
         var ch2 = state.CurrentSource.Length >= 2 ? state.CurrentSource[1] : '\0';
         if (main)
         {
            switch (ch)
            {
               case '\\' when ch2 == '\'':
                  escaped = true;
                  break;
               case '\'' when escaped:
                  pattern.Append('\'');
                  escaped = false;
                  break;
               case '\'':
                  state.AddToken(Color.Regex);
                  state.Move(1);
                  pattern.Append("; u");
                  builder.Add(new RegexSymbol(pattern.ToString(), ignoreCase, multiline, global, textOnly));
                  return unit;
               case ';' when ch2 == '\'':
                  escaped = true;
                  break;
               case ';' when escaped:
                  pattern.Append(';');
                  escaped = false;
                  break;
               case ';':
                  main = false;
                  break;
               default:
                  pattern.Append(ch);
                  break;
            }
         }
         else
         {
            switch (ch)
            {
               case 'i' or 'I':
               {
                  ignoreCase = true;
                  break;
               }
               case 'm' or 'M':
               {
                  multiline = true;
                  break;
               }
               case 'g' or 'G':
               {
                  global = true;
                  break;
               }
               case 't' or 'T':
               {
                  textOnly = true;
                  break;
               }
               case ' ':
                  break;
               case '\'':
                  state.AddToken(Color.Regex);
                  state.Move(1);
                  pattern.Append('u');
                  builder.Add(new RegexSymbol(pattern.ToString(), ignoreCase, multiline, global, textOnly));
                  return unit;
            }
         }

         state.AddToken(Color.Regex);
         state.Move(1);
      }

      return fail("Open regex");
   }
}