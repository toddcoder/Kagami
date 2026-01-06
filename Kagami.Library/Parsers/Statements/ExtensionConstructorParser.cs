using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class ExtensionConstructorParser(string className) : StatementParser
{
   [GeneratedRegex(@"^(\s*)(init)(\()")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.OpenParenthesis);
      state.CreateReturnType();
      var _parametersBlock =
         from parametersValue in getParameters(state)
         from blockValue in getAnyBlock(state)
         select (parametersValue, blockValue);
      if (_parametersBlock is (true, var (parameters, block)))
      {
         state.RemoveReturnType();
         var function = new Function(className, parameters, false, block, false, false, "");
         state.AddStatement(function);

         return unit;
      }
      else
      {
         return _parametersBlock.Exception;
      }
   }
}