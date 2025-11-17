using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.CommonFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class ConvertParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(convert|implicit)(\s+)({REGEX_CLASS})(\()({REGEX_FIELD})(\s+)({REGEX_CLASS})(\))")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var type = tokens[1].Text;
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


         if (type == "convert")
         {
            var functionName = convertFunctionName(fromClass, toClass);
            var parameter = new Parameter(false, false, "", parameterName, nil, nil, false, false);
            var parameters = new Parameters(parameter);
            var function = new Function(functionName, parameters, false, block, false, false, "");
            state.AddStatement(function);
            Module.Global.Value.RegisterConversion(fromClass, toClass, $"{functionName}(_)");
         }
         else
         {
            state.AddStatement(new AutoConversion(parameterName, fromClass, toClass, block));
         }
      }

      return unit;
   }
}