using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class DeclareNewFieldParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(var|let)(\s+)({REGEX_FIELD})(\s+)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.BeginTransaction();

      var mutable = tokens[2].Text == "var";
      var fieldName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Whitespace);

      var _className =
         from possibleTypeConstraint in parseTypeConstraint(state)
         from typeConstraint in possibleTypeConstraint.Maybe
         select typeConstraint.Comparisands[0].Name;
      if (_className is (true, var className))
      {
         state.AddStatement(new DefineNewField(mutable, fieldName, className));
         state.CommitTransaction();

         return unit;
      }

      state.RollBackTransaction();
      return nil;
   }
}