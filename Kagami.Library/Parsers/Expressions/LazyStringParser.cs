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

      List<LazyStringPart> list = [];
      var text = new StringBuilder();
      var type = LazyStringSegment.String;
      var startIndex = state.Index;
      var sourceIndex = 0;

      while (state.More)
      {
         var ch = state.CurrentSource[sourceIndex++];
         switch (ch)
         {
            case '"' when type is LazyStringSegment.Escaped:
               text.Append(ch);
               type = LazyStringSegment.String;
               break;
            case '"' when type is LazyStringSegment.Hex:
            {
               var _matchedChar = fromHex(text.ToString());
               if (_matchedChar is (true, var matchedChar))
               {
                  list.Add(new LazyStringPart.Hex(matchedChar.ToString()));
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
            case '\\' when type is LazyStringSegment.Escaped:
               text.Append(ch);
               type = LazyStringSegment.String;
               break;
            case '\\':
               type = LazyStringSegment.Escaped;
               break;
            case 'n' when type is LazyStringSegment.Escaped:
               text.Append('\n');
               type = LazyStringSegment.String;
               break;
            case 'n':
               text.Append(ch);
               break;
            case 'r' when type is LazyStringSegment.Escaped:
               text.Append('\r');
               type = LazyStringSegment.String;
               break;
            case 'r':
               text.Append(ch);
               break;
            case 't' when type is LazyStringSegment.Escaped:
               text.Append('\t');
               type = LazyStringSegment.String;
               break;
            case 't':
               text.Append(ch);
               break;
            case 'u' when type is LazyStringSegment.Escaped:
               list.Add(new LazyStringPart.String(text.ToString()));
               type = LazyStringSegment.Hex;
               text.Clear();
               text.Append(ch);
               break;
            case 'u':
               text.Append(ch);
               break;
            case '{' when type is LazyStringSegment.Escaped:
               list.Add(new LazyStringPart.String(text.ToString()));
               type = LazyStringSegment.Hex;
               text.Clear();
               text.Append(ch);
               break;
            case '{':
               text.Append(ch);
               break;
            case '$' when type is LazyStringSegment.Escaped:
               text.Append(ch);
               type = LazyStringSegment.String;
               break;
            case '$':
               list.Add(new LazyStringPart.String(text.ToString()));
               text.Clear();
               text.Append(ch);
               type = LazyStringSegment.Field;
               break;
            case >= 'A' and <= 'Z' or >= 'a' and <= 'z' or '`' or '_' or >= '0' and <= '9' when type is LazyStringSegment.Field:
               text.Append(ch);
               break;
            case '[' when type is LazyStringSegment.Field:
               list.Add(new LazyStringPart.Field(text.ToString()));
               text.Clear();
               type = LazyStringSegment.Format;
               break;
            case ']' when type is LazyStringSegment.Format:
               list.Add(new LazyStringPart.Format(text.ToString()));
               text.Clear();
               type = LazyStringSegment.String;
               break;
            default:
               switch (type)
               {
                  case LazyStringSegment.Field:
                     list.Add(new LazyStringPart.Field(text.ToString()));
                     text.Clear();
                     text.Append(ch);
                     type = LazyStringSegment.String;
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
               case LazyStringSegment.String:
                  list.Add(new LazyStringPart.String(text.ToString()));
                  break;
               case LazyStringSegment.Escaped:
                  return fail("Can't have a pending escape");
               case LazyStringSegment.Field:
                  list.Add(new LazyStringPart.Field(text.ToString()));
                  break;
               case LazyStringSegment.Format:
                  return fail("Can't have a pending format");
               case LazyStringSegment.Hex:
                  list.Add(new LazyStringPart.Hex(text.ToString()));
                  break;
            }
         }

         var lazyString = new StringBuilder();
         var index = startIndex;
         foreach (var part in list)
         {
            switch (part)
            {
               case LazyStringPart.Field field:
                  state.Colorize(index, field.Text, Color.Identifier);
                  index += field.Text.Length;
                  state.Move(field.Text.Length);
                  lazyString.Append(field.Text);
                  break;
               case LazyStringPart.Format format:
                  state.Colorize(index, format.Text, Color.Format);
                  index += format.Text.Length;
                  state.Move(format.Text.Length);
                  lazyString.Append(format.Text);
                  break;
               case LazyStringPart.Hex hex:
                  state.Colorize(index, hex.Text, Color.String);
                  index += hex.Text.Length;
                  state.Move(hex.Text.Length);
                  lazyString.Append(hex.Text);
                  break;
               case LazyStringPart.String @string:
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