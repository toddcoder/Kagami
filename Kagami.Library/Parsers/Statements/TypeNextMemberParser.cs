using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class TypeNextMemberParser(Maybe<IObject> _previousOrdinal) : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(,)(\s*)({REGEX_CLASS})(\()?")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var className = tokens[4].Text;
      var hasParameters = tokens[5].Text == "(";
      state.Colorize(tokens, Color.Whitespace, Color.Structure, Color.Whitespace, Color.Class, Color.OpenParenthesis);

      var _enumMember = TypeMemberParser.ParseTypeMember(state, className, hasParameters, _previousOrdinal);
      if (_enumMember is (true, var (enumMemberData, _ordinal)))
      {
         TypeMemberData = enumMemberData;
         Ordinal = _ordinal;

         return unit;
      }
      else
      {
         return _enumMember.Exception;
      }
   }

   public Maybe<TypeMemberData> TypeMemberData { get; set; } = nil;

   public Maybe<IObject> Ordinal { get; set; } = nil;
}