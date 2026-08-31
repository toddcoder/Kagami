using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class PatternsMemberParser(Parameters parameters) : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(\|)(\s*)({REGEX_CLASS})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var patternName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Whitespace, Color.Class);
      state.CreateReturnType();

      var _block = getAnyBlock(state);
      if (_block is (true, var block))
      {
         state.RemoveReturnType();
         state.RegisterPattern(patternName);
         state.AddStatement(new PatternStatement(patternName, parameters, block));

         return unit;
      }
      else
      {
         state.RemoveReturnType();
         return _block.Exception;
      }
   }
}