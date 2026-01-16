using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class MixinParser(ClassBuilder builder) : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(mix)(\s+)({REGEX_CLASS})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var mixinName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class);
      if (Module.GetMixin(mixinName) is (true, var metaClass))
      {
         builder.AddMixin(metaClass);
         return unit;
      }
      else
      {
         return mixinNotFound(mixinName);
      }
   }
}