using System.Text.RegularExpressions;
using Core.Matching;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Expressions;

public partial class PlaceholderParser : SymbolParser
{
   public PlaceholderParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex($@"^(\s*)((?:use|var)\s+)?({REGEX_FIELD})\b(?!"")")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var mutable = tokens[2].Text.Trim();
      var placeholderName = tokens[3].Text;
      if (placeholderName is "false" or "true" or "nil")
      {
         return nil;
      }

      if (placeholderName.StartsWith('`'))
      {
         state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Identifier);
         builder.Add(new FieldSymbol(placeholderName));
         return unit;
      }

      if (placeholderName.IsMatch("^ ['A-Z']"))
      {
         state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Invokable);
         if (state.LookAhead(@"^\("))
         {
            state.Scan(@"^(\()", Color.OpenParenthesis);
            var newFlags = builder.Flags.Clone();
            newFlags[ExpressionFlags.InSubExpression] = false;
            var _arguments = getArguments(state, newFlags);
            if (_arguments is (true, var arguments))
            {
               if (state.LookAhead(IsParser.REGEX_FIELD_NAME, 2) is (true, var word) && !isAKeyword(word))
               {
                  var _fieldResult = state.Scan(IsParser.REGEX_FIELD_NAME, Color.Whitespace, Color.Identifier);
                  if (_fieldResult is (true, var fieldName))
                  {
                     var expression =
                        new Expression(new NameValueSymbol(fieldName.Trim(),
                           new Expression(new InvokeSymbol(placeholderName, arguments, nil, true))));
                     builder.Add(expression);
                  }
                  else
                  {
                     return _fieldResult.Exception;
                  }
               }
               else
               {
                  builder.Add(new InvokeSymbol(placeholderName, arguments, nil, true));
               }

               return unit;
            }
            else
            {
               return _arguments.Exception;
            }
         }
         else
         {
            if (state.LookAhead(IsParser.REGEX_FIELD_NAME, 2) is (true, var word) && !isAKeyword(word))
            {
               var _fieldResult = state.Scan(IsParser.REGEX_FIELD_NAME, Color.Whitespace, Color.Identifier);
               if (_fieldResult is (true, var fieldName))
               {
                  var expression =
                     new Expression(new NameValueSymbol(fieldName.Trim(), new Expression(new ClassSymbol(placeholderName))));
                  builder.Add(expression);
               }
               else
               {
                  return _fieldResult.Exception;
               }
            }
            else if (Protocols.Protocols.Get(placeholderName))
            {
               builder.Add(new PushObjectSymbol(new ProtocolConstraint(placeholderName)));
            }
            else
            {
               builder.Add(new ClassSymbol(placeholderName));
            }

            return unit;
         }
      }

      var name = mutable switch
      {
         "use" => placeholderName,
         "var" => $"+{placeholderName}",
         _ => $"-{placeholderName}"
      };
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Identifier);
      builder.Add(new PlaceholderSymbol(name));

      return unit;
   }
}