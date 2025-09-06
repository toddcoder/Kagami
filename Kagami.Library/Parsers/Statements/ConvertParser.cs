using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.CommonFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class ConvertParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(convert)(\s+)({REGEX_CLASS})(\()({REGEX_FIELD})(\s+)({REGEX_CLASS})(\))")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var toClass = tokens[4].Text;
      var parameterName = tokens[6].Text;
      var (fromClass, color) = getClassNameWithColor(tokens[8].Text);
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class, Color.OpenParenthesis, Color.Identifier,
         Color.Whitespace, color, Color.CloseParenthesis);

      state.CreateYieldFlag();
      state.CreateReturnType();

      var _block = getAnyBlock(state);
      if (_block is (true, var block))
      {
         state.RemoveYieldFlag();
         state.RemoveReturnType();

         var functionName = convertFunctionName(fromClass, toClass);
         var parameter = new Parameter(false, "", parameterName, nil, nil, false, false);
         var parameters = new Parameters(parameter);
         var function = new Function(functionName, parameters, block, false, false, "");
         state.AddStatement(function);
         Module.Global.Value.RegisterConversion(fromClass, toClass, $"{functionName}(_)");
      }

      return unit;
   }
}