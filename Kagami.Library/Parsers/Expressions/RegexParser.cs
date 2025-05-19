using System.Text;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class RegexParser : SymbolParser
{
   public RegexParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override string Pattern => @"^ /(|s|) /'\'";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Structure);

      var pattern = new StringBuilder();
      var type = RegexParsingType.Outside;
      var ignoreCase = false;
      var multiline = false;
      var global = false;
      var textOnly = false;

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
                     builder.Add(new RegexSymbol(pattern.ToString(), ignoreCase, multiline, global, textOnly));

                     return unit;
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
            case RegexParsingType.WaitingForSingleQuote:
               type = ch switch
               {
                  '\'' => RegexParsingType.Outside,
                  '\\' => RegexParsingType.EscapedSingleQuote,
                  _ => type
               };

               pattern.Append(ch);
               state.AddToken(Color.String);
               break;
            case RegexParsingType.WaitingForDoubleQuote:
               type = ch switch
               {
                  '"' => RegexParsingType.Outside,
                  '\\' => RegexParsingType.EscapedDoubleQuote,
                  _ => type
               };

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