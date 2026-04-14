using Core.Monads;
using Core.Strings;
using Kagami.Library.Objects;
using Kagami.Library.Parsers.Expressions;
using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class EnumMemberParser(IObject value) : StatementParser
{
   [GeneratedRegex($@"^(\s*)({REGEX_FIELD})(\s*=\s*)?")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var currentValue = value;

      var enumMember = tokens[2].Text;
      var expectValue = tokens[3].Text.IsNotEmpty();
      state.Colorize(tokens, Color.Whitespace, Color.Identifier, Color.Structure);

      if (expectValue)
      {
         var _expression = getExpression(state, ExpressionFlags.Standard | ExpressionFlags.OmitComma);
         if (_expression is (true, var expression))
         {
            var firstSymbol = expression.Symbols[0];
            if (firstSymbol is IConstant { Object: IRangeItem rangeItem })
            {
               currentValue = (IObject)rangeItem;
            }
            else
            {
               return fail("Supplied ordinal isn't a range item");
            }
         }
         else
         {
            return _expression.Exception;
         }
      }

      EnumMemberData = new EnumMemberData(enumMember, currentValue);

      return unit;
   }

   public Maybe<EnumMemberData> EnumMemberData { get; set; } = nil;
}