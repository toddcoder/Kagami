using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Parsers.Expressions;
using System.Text.RegularExpressions;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class EnumMemberParser2(string enumClassName) : StatementParser
{
   [GeneratedRegex($@"^(\s*)(when)(\s+)({REGEX_CLASS})(\()?")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var className = $"{enumClassName}${tokens[4].Text}";
      var hasParameters = tokens[5].Text == "(";
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class, Color.OpenParenthesis);

      Module.Global.Value.ForwardReference(className);

      Parameters parameters;
      if (hasParameters)
      {
         var _parameters = getParameters(state);
         if (_parameters)
         {
            parameters = _parameters;
         }
         else if (_parameters.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            parameters = new Parameters(0);
         }
      }
      else
      {
         parameters = Parameters.Empty;
      }

      Maybe<IObject> _ordinal = nil;
      var _ordinalScan =
         from equal in state.Scan(@"^(\s*)(=)", Color.Whitespace, Color.Structure)
         from value in getExpression(state, ExpressionFlags.Standard)
         select value;
      if (_ordinalScan is (true, var expression))
      {
         var firstSymbol = expression.Symbols[0];
         if (firstSymbol is IConstant { Object: IRangeItem rangeItem })
         {
            _ordinal = ((IObject)rangeItem).Some();
         }
         else
         {
            return fail("Supplied ordinal isn't a range item");
         }
      }
      else if (_ordinalScan.Exception is (true, var exception))
      {
         return exception;
      }

      var _block = getBlock(state).Maybe();

      EnumMemberData = new EnumMemberData(className, enumClassName, parameters, _ordinal, _block);

      return unit;
   }

   public Maybe<EnumMemberData> EnumMemberData { get; set; } = nil;
}