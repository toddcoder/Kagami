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

public partial class EnumMemberParser2(string enumClassName, Maybe<IObject> _previousOrdinal) : StatementParser
{
   [GeneratedRegex($@"^(\s*)(when)(\s+)({REGEX_CLASS})(\()?")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var shortName = tokens[4].Text;
      var className = $"{enumClassName}${shortName}";
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

      if (_previousOrdinal is (true, var previousOrdinal))
      {
         if (previousOrdinal is IRangeItem rangeItem)
         {
            _ordinal = ((IObject)rangeItem.Successor).Some();
         }
         else
         {
            return fail($"{previousOrdinal.Image} must be a range item");
         }
      }
      else
      {
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
      }

      Ordinal = _ordinal;

      var _block = getBlock(state).Maybe();

      Module.Global.Value.ForwardReference(enumClassName);
      Module.Global.Value.ForwardReference(shortName);
      EnumMemberData = new EnumMemberData(className, enumClassName, parameters, _ordinal, _block);

      return unit;
   }

   public Maybe<EnumMemberData> EnumMemberData { get; set; } = nil;

   public Maybe<IObject> Ordinal { get; set; } = nil;
}