using System.Text;
using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Expressions;

public partial class LazyStringParser : SymbolParser
{
   public LazyStringParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(l)("")")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.StringPart, Color.String);

      List<StringPart> list = [];
      var text = new StringBuilder();
      var type = StringSegment.String;
      var startIndex = state.Index;
      var sourceIndex = 0;

      while (state.More)
      {
         var ch = state.CurrentSource[sourceIndex++];
         switch (ch)
         {
            case '"' when type is StringSegment.Escaped:
               text.Append(ch);
               type = StringSegment.String;
               break;
            case '"' when type is StringSegment.Hex:
            {
               var _matchedChar = fromHex(text.ToString());
               if (_matchedChar is (true, var matchedChar))
               {
                  list.Add(new StringPart.Hex(matchedChar.ToString()));
               }
               else if (_matchedChar.Exception is (true, var exception))
               {
                  return exception;
               }
               else
               {
                  return badHex(text.ToString());
               }

               text.Clear();
               text.Append(ch);
               return getLazyString();
            }
            case '"':
               return getLazyString();
            case '\\' when type is StringSegment.Escaped:
               text.Append(ch);
               type = StringSegment.String;
               break;
            case '\\':
               type = StringSegment.Escaped;
               break;
            case 'n' when type is StringSegment.Escaped:
               text.Append('\n');
               type = StringSegment.String;
               break;
            case 'n':
               text.Append(ch);
               break;
            case 'r' when type is StringSegment.Escaped:
               text.Append('\r');
               type = StringSegment.String;
               break;
            case 'r':
               text.Append(ch);
               break;
            case 't' when type is StringSegment.Escaped:
               text.Append('\t');
               type = StringSegment.String;
               break;
            case 't':
               text.Append(ch);
               break;
            case 'u' when type is StringSegment.Escaped:
               list.Add(new StringPart.String(text.ToString()));
               type = StringSegment.Hex;
               text.Clear();
               text.Append(ch);
               break;
            case 'u':
               text.Append(ch);
               break;
            case '{' when type is StringSegment.Escaped:
               list.Add(new StringPart.String(text.ToString()));
               type = StringSegment.Hex;
               text.Clear();
               text.Append(ch);
               break;
            case '{':
               text.Append(ch);
               break;
            case '$' when type is StringSegment.Escaped:
               text.Append(ch);
               type = StringSegment.String;
               break;
            case '$':
               list.Add(new StringPart.String(text.ToString()));
               text.Clear();
               text.Append(ch);
               type = StringSegment.Field;
               break;
            case >= 'A' and <= 'Z' or >= 'a' and <= 'z' or '`' or '_' or >= '0' and <= '9' when type is StringSegment.Field:
               text.Append(ch);
               break;
            case '[' when type is StringSegment.Field:
               list.Add(new StringPart.Field(text.ToString()));
               text.Clear();
               type = StringSegment.Format;
               break;
            case ']' when type is StringSegment.Format:
               list.Add(new StringPart.Format(text.ToString()));
               text.Clear();
               type = StringSegment.String;
               break;
            default:
               switch (type)
               {
                  case StringSegment.Field:
                     list.Add(new StringPart.Field(text.ToString()));
                     text.Clear();
                     text.Append(ch);
                     type = StringSegment.String;
                     break;
                  default:
                     text.Append(ch);
                     break;
               }

               break;
         }
      }

      return openString();

      Optional<Unit> getLazyString()
      {
         if (text.Length > 0)
         {
            switch (type)
            {
               case StringSegment.String:
                  list.Add(new StringPart.String(text.ToString()));
                  break;
               case StringSegment.Escaped:
                  return fail("Can't have a pending escape");
               case StringSegment.Field:
                  list.Add(new StringPart.Field(text.ToString()));
                  break;
               case StringSegment.Format:
                  return fail("Can't have a pending format");
               case StringSegment.Hex:
                  list.Add(new StringPart.Hex(text.ToString()));
                  break;
            }
         }

         var lazyString = new StringBuilder();
         var index = startIndex;
         foreach (var part in list)
         {
            switch (part)
            {
               case StringPart.Field field:
                  state.Colorize(index, field.Text, Color.Identifier);
                  index += field.Text.Length;
                  state.Move(field.Text.Length);
                  lazyString.Append(field.Text);
                  break;
               case StringPart.Format format:
                  state.Colorize(index, format.Text, Color.Format);
                  index += format.Text.Length;
                  state.Move(format.Text.Length);
                  lazyString.Append(format.Text);
                  break;
               case StringPart.Hex hex:
                  state.Colorize(index, hex.Text, Color.String);
                  index += hex.Text.Length;
                  state.Move(hex.Text.Length);
                  lazyString.Append(hex.Text);
                  break;
               case StringPart.String @string:
                  state.Colorize(index, @string.Text, Color.String);
                  index += @string.Text.Length;
                  state.Move(@string.Text.Length);
                  lazyString.Append(@string.Text);
                  break;
            }
         }

         state.Colorize(index, '"', Color.String);
         state.Move(1);
         builder.Add(new LazyStringSymbol(lazyString.ToString()));

         return unit;
      }
   }
}