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

public partial class EnumMemberParser(string enumClassName, Maybe<IObject> _previousOrdinal) : StatementParser
{
   public static Optional<(TypeMemberData, Maybe<IObject>)> ParseEnumMember(ParseState state, string className, bool hasParameters,
      string enumClassName, Maybe<IObject> _previousOrdinal)
   {
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
         from value in getExpression(state, ExpressionFlags.Standard | ExpressionFlags.OmitComma)
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

      if (!_ordinal && _previousOrdinal is (true, var previousOrdinal))
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

      var _block = getBlock(state).Maybe();

      Module.Global.Value.ForwardReference(className);
      var enumMemberData = new TypeMemberData(className, parameters, _ordinal, _block);

      return (enumMemberData, _ordinal);
   }

   [GeneratedRegex($@"^(\s*)(\|)(\s*)({REGEX_CLASS})(\()?")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var className = tokens[4].Text;
      var hasParameters = tokens[5].Text == "(";
      state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Whitespace, Color.Class, Color.OpenParenthesis);

      var _enumMember = ParseEnumMember(state, className, hasParameters, enumClassName, _previousOrdinal);
      if (_enumMember is (true, var (enumMemberData, _ordinal)))
      {
         EnumMemberData = enumMemberData;
         Ordinal = _ordinal;

         return unit;
      }
      else
      {
         return _enumMember.Exception;
      }
   }

   public Maybe<TypeMemberData> EnumMemberData { get; set; } = nil;

   public Maybe<IObject> Ordinal { get; set; } = nil;
}