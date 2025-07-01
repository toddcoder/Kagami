using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Runtime;
using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Parsers.Expressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Class = Kagami.Library.Nodes.Statements.Class;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class EnumMemberParser(string enumClassName, Block commonBlock, Maybe<IRangeItem> _previousOrdinal) : StatementParser
{
   [GeneratedRegex($@"^(\s*)(when)(\s+)({REGEX_CLASS})(\()?")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var className = tokens[4].Text;
      var hasParameters = tokens[5].Text == "(";
      HasParameters = hasParameters;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class, Color.OpenParenthesis);

      className = $"{enumClassName}${className}";
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

      Maybe<IRangeItem> _ordinal;
      var _ordinalScan =
         from equal in state.Scan(@"^(\s*)(=)", Color.Whitespace, Color.Structure)
         from value in getExpression(state, ExpressionFlags.Standard)
         select value;
      if (_ordinalScan is (true, var expression))
      {
         var firstSymbol = expression.Symbols[0];
         if (firstSymbol is IConstant { Object: IRangeItem rangeItem })
         {
            _ordinal = rangeItem.Some();
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
      else
      {
         _ordinal = _previousOrdinal.Map(po => po.Successor);
      }

      /*var builder = new EnumMemberClassBuilder(className, parameters, enumClassName, commonBlock)
      {
         Selector = parameters.Selector(className),
         Ordinal = _ordinal.Map(o => (IObject)o)
      };*/
      /*var _registered = builder.Register();
      if (_registered)
      {
         var cls = new Class(builder);
         state.AddStatement(cls);

         Ordinal = _ordinal;

         Matching = cls;
      }
      else
      {
         return _registered.Exception;
      }*/

      return unit;
   }

   public Maybe<Class> Matching { get; set; } = nil;

   public Maybe<IRangeItem> Ordinal { get; set; } = nil;

   public bool HasParameters { get; set; }
}