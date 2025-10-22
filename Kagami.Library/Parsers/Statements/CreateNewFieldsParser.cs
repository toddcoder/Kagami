using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class CreateNewFieldsParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(var)(\s+)({REGEX_FIELD})( *,)")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var field1 = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Structure);

      List<string> fields = [field1];

      while (state.More)
      {
         var _field = state.Scan($@"^(\s*)({REGEX_FIELD})( *,)", 2, Color.Whitespace, Color.Identifier, Color.Structure);
         if (_field is (true, var field))
         {
            fields.Add(field);
         }
         else
         {
            break;
         }
      }

      var _lastField = state.Scan(@$"^(\s*)({REGEX_FIELD})\b", 2, Color.Whitespace, Color.Identifier);
      if (_lastField is (true, var lastField))
      {
         fields.Add(lastField);
      }
      else
      {
         return fail("Last field not found");
      }

      var _className = state.Scan($@"^(\s+)({REGEX_CLASS_OR_ALIAS})\b", 2, Color.Whitespace, Color.Class);
      if (_className is (true, var className))
      {
         (className, var color) = getClassNameWithColor(className);
         state.Tokens[^1].Color = color;
         state.AddStatement(new CreateNewFields([.. fields], className));

         return unit;
      }
      else
      {
         return fail("Class not provided");
      }
   }
}