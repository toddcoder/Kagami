using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class AssignFromBlockParser : StatementParser
{
   //public override string Pattern => $"^ (/('var' | 'let') /(/s+))? /({REGEX_FIELD}) /b";

   [GeneratedRegex($@"^(?:(var|let)(\s+))?({REGEX_FIELD})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.BeginTransaction();

      var isNew = tokens[1].Text.IsNotEmpty();
      var mutable = tokens[1].Text == "var";
      var fieldName = tokens[3].Text;
      state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Identifier);

      var _result =
         from typeConstraintValue in parseTypeConstraint(state)
         from scanned in state.Scan($@"^(\s*)(=)({REGEX_EOL})", Color.Whitespace, Color.Structure, Color.Whitespace)
         from blockValue in getBlock(state)
         select (typeConstraintValue, blockValue);

      if (_result is (true, var (typeConstraint, block)))
      {
         state.AddStatement(new AssignToFieldWithBlock(isNew, mutable, fieldName, typeConstraint.Maybe, block));
         state.CommitTransaction();

         return unit;
      }
      else
      {
         state.RollBackTransaction();
         return nil;
      }
   }
}