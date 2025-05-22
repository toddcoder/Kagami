using System.Collections.Generic;
using System.Text;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.Numbers;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class InterpolatedStringParser : SymbolParser
{
   public InterpolatedStringParser(ExpressionBuilder builder) : base(builder) { }

   public override string Pattern => "^ /(/s*) /['$f'] /[dquote]";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var isFailure = tokens[2].Text == "f";
      state.Colorize(tokens, Color.Whitespace, Color.StringPart, Color.String);

      Maybe<string> _firstString = nil;
      List<Expression> expressions = [];
      List<string> suffixes = [];
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
               {
                  var _fromHex1 = fromHex(hexText.ToString());
                  if (_fromHex1 is (true, var fromHex1))
                  {
                     text.Append(fromHex1);
                  }
                  else if (_fromHex1.Exception is (true, var exception))
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

               var symbol = _firstString.Map(Symbol (prefix) =>
               {
                  suffixes.Add(text.ToString());
                  var expressionsArray = expressions.ToArray();
                  var suffixesArray = suffixes.ToArray();

                  return new InterpolatedStringSymbol(prefix, expressionsArray, suffixesArray, isFailure);
               }) | (() => new StringSymbol(text.ToString(), isFailure));
               builder.Add(symbol);

               return unit;
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
               state.AddToken(index + length, 1, Color.OpenParenthesis);

               if (_firstString)
               {
                  suffixes.Add(text.ToString());
               }
               else
               {
                  _firstString = text.ToString();
               }

               text.Clear();

               var _expression = getExpression(state, "^ /')'", builder.Flags, Color.CloseParenthesis);
               if (_expression is (true, var expression))
               {
                  expressions.Add(expression);
                  index = state.Index;
                  length = 0;
                  continue;
               }
               else if (_expression.Exception is (true, var exception))
               {
                  return exception;
               }
               else
               {
                  return expectedExpression();
               }
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
               {
                  if (ch.Between('0').And('9') || ch.Between('a').And('f') && hexText.Length < 6)
                  {
                     hexText.Append(ch);
                  }
                  else
                  {
                     var _fromHex2 = fromHex(hexText.ToString());
                     if (_fromHex2 is (true, var fromHex2))
                     {
                        hexText.Append(fromHex2);
                        hexText.Append(ch);
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
               }
               else
               {
                  text.Append(ch);
               }

               escaped = false;
            }
               break;
         }

         length++;
         state.Move(1);
      }

      return openString();
   }
}