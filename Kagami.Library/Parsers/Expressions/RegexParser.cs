using System.Text;
using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class RegexParser : SymbolParser
{
   public RegexParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(x"")")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Regex);

      var pattern = new StringBuilder();
      var type = RegexParsingType.Outside;
      var ignoreCase = false;
      var multiline = false;
      var global = false;
      var textOnly = false;
      var escaped = false;

      while (state.More)
      {
         var ch = state.CurrentSource[0];

         switch (type)
         {
            case RegexParsingType.Outside:
               switch (ch)
               {
                  case '\\' when escaped:
                     pattern.Append(ch);
                     state.AddToken(Color.String);
                     escaped = false;
                     break;
                  case '\\':
                     escaped = true;
                     break;
                  case '"' when escaped:
                     pattern.Append('"');
                     state.AddToken(Color.String);
                     escaped = false;
                     break;
                  case '"':
                     state.AddToken(Color.Regex);
                     state.Move(1);
                     builder.Add(new RegexSymbol(pattern.ToString(), ignoreCase, multiline, global, textOnly));

                     return unit;
                  case '\'':
                     pattern.Append(ch);
                     state.AddToken(Color.String);
                     type = RegexParsingType.SingleQuote;
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
                           {
                              color = Color.Number;
                           }
                           else if (char.IsLetter(ch))
                           {
                              color = Color.Identifier;
                           }

                           break;
                     }

                     state.AddToken(color);
                     break;
               }

               break;
            case RegexParsingType.SingleQuote:
               switch (ch)
               {
                  case '\\' when escaped:
                     pattern.Append(ch);
                     state.AddToken(Color.String);
                     escaped = false;
                     break;
                  case '\\':
                     escaped = true;
                     break;
                  case '\'' when escaped:
                     pattern.Append('\'');
                     state.AddToken(Color.String);
                     escaped = false;
                     break;
                  case '\'':
                     pattern.Append('\'');
                     state.AddToken(Color.String);
                     type = RegexParsingType.Outside;
                     break;
                  default:
                     pattern.Append(ch);
                     state.AddToken(Color.String);
                     break;
               }
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
                  case 't':
                  case 'T':
                     textOnly = true;
                     break;
                  case '\\':
                     state.AddToken(Color.Structure);
                     state.Move(1);
                     builder.Add(new RegexSymbol(pattern.ToString(), ignoreCase, multiline, global, textOnly));
                     return unit;
                  default:
                     return fail($"Didn't understand option '{ch}'");
               }

               state.AddToken(Color.Symbol);
               break;
         }

         state.Move(1);
      }

      return fail("Open regex");
   }
}