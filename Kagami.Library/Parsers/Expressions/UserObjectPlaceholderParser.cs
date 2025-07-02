using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Expressions;

public partial class UserObjectPlaceholderParser : SymbolParser
{
   public UserObjectPlaceholderParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex($@"^(\s*)({REGEX_CLASS})(\()")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var name = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Class, Color.OpenParenthesis);

      var _parameters = getParameters(state);
      if (_parameters is (true, var parameters))
      {
         var userObjectSymbol = new UserObjectPlaceholder(name, [..parameters.Select(p => p.Name)]);
         builder.Add(new PushObjectSymbol(userObjectSymbol));

         return unit;
      }
      else
      {
         return _parameters.Exception;
      }
   }
}