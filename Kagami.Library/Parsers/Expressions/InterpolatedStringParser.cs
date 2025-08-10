using System.Text;
using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.Numbers;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class InterpolatedStringParser : SymbolParser
{
   public InterpolatedStringParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)([\$f])([""])")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var isFailure = tokens[2].Text == "f";
      state.Colorize(tokens, Color.Whitespace, Color.StringPart, Color.String);

      Maybe<string> _firstString = nil;
      List<Expression> expressions = [];
      List<string> formats = [];
      List<string> suffixes = [];
      var text = new StringBuilder();
      var hexText = new StringBuilder();
      var fieldText = new StringBuilder();
      var type = StringSegment.String;
      var index = state.Index;
      var length = 0;

      while (state.More)
      {
         var ch = state.CurrentSource[0];
         switch (ch)
         {
            case '"' when type is StringSegment.Escaped:
               text.Append(ch);
               break;
            case '"':
            {
               switch (type)
               {
                  case StringSegment.Hex:
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

                     break;
                  }
                  case StringSegment.Field:
                     text.Append(ch);
                     break;
               }

               state.Move(1);
               state.AddToken(index, length + 1, Color.String);

               var symbol = _firstString.Map(Symbol (prefix) =>
               {
                  suffixes.Add(text.ToString());
                  Expression[] expressionsArray = [.. expressions];
                  string[] formatsArray = [.. formats];
                  string[] suffixesArray = [.. suffixes];

                  return new InterpolatedStringSymbol(prefix, expressionsArray, formatsArray, suffixesArray, isFailure);
               }) | (() => new StringSymbol(text.ToString(), isFailure));
               builder.Add(symbol);

               return unit;
            }
            case '(' when type is StringSegment.Escaped:
               text.Append(ch);
               type = StringSegment.String;
               break;
            case '(':
            {
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

               var _expression = getExpression(state, @"^(\))", builder.Flags, Color.CloseParenthesis);
               if (_expression is (true, var expression))
               {
                  expressions.Add(expression);
                  index = state.Index;
                  length = 0;

                  var _format = state.ScanFormat();
                  if (_format is (true, var format))
                  {
                     formats.Add(format);
                     index = state.Index;
                     length = 0;
                  }
                  else
                  {
                     formats.Add("");
                  }

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
            case '\\' when type is StringSegment.Escaped:
               text.Append(ch);
               type = StringSegment.String;
               break;
            case '\\':
               type = StringSegment.Escaped;
               break;
            case >= 'A' and <= 'Z' or >= 'a' and <= 'z' or '`' or '_' or >= '0' and <= '9' when type is StringSegment.Field:
               fieldText.Append(ch);
               break;
            case 'n' when type is StringSegment.Escaped:
               text.Append(ch);
               type = StringSegment.String;
               break;
            case 'n':
               text.Append('n');
               break;
            case 'r' when type is StringSegment.Escaped:
               text.Append(ch);
               type = StringSegment.String;
               break;
            case 'r':
               text.Append('r');
               break;
            case 't' when type is StringSegment.Escaped:
               text.Append(ch);
               type = StringSegment.String;
               break;
            case 't':
               text.Append('t');
               break;
            case 'u' when type is StringSegment.Escaped:
               text.Append(ch);
               type = StringSegment.Escaped;
               break;
            case 'u':
               text.Append('u');
               break;
            case '$' when type is StringSegment.Escaped:
               text.Append(ch);
               type = StringSegment.String;
               break;
            case '$':
               type = StringSegment.Field;
               break;
            default:
            {
               switch (type)
               {
                  case StringSegment.Field:
                  {
                     var fieldSymbol = new FieldSymbol(fieldText.ToString());
                     var expression = new Expression(fieldSymbol);
                     expressions.Add(expression);
                     var _format = state.ScanFormat();
                     if (_format is (true, var format))
                     {
                        formats.Add(format);
                        index = state.Index;
                        length = 0;
                     }
                     else
                     {
                        formats.Add("");
                        text.Append(ch);
                     }

                     type = StringSegment.String;
                     break;
                  }
                  case StringSegment.Escaped when ch.Between('0').And('9') || ch.Between('a').And('f') && hexText.Length < 6:
                     type = StringSegment.Hex;
                     hexText.Append(ch);
                     break;
                  case StringSegment.Escaped:
                  {
                     var _fromHex2 = fromHex(hexText.ToString());
                     if (_fromHex2 is (true, var fromHex2))
                     {
                        hexText.Append(fromHex2);
                        hexText.Append(ch);
                        type = StringSegment.Hex;
                     }
                     else if (_fromHex2.Exception is (true, var exception))
                     {
                        return exception;
                     }
                     else
                     {
                        return badHex(hexText.ToString());
                     }

                     break;
                  }
                  default:
                     text.Append(ch);
                     break;
               }
            }
               break;
         }

         length++;
         state.Move(1);
      }

      return openString();
   }
}