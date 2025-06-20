﻿using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class DefAssignParser : EndingInExpressionParser
{
   protected string fieldName = "";

   [GeneratedRegex($@"^(\s*)(def)(\s+)({REGEX_FIELD})(\s*)(=)(?!=)")]
   public override partial Regex Regex();

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      fieldName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Structure);

      return unit;
   }

   public override Optional<Unit> Suffix(ParseState state, Expression expression)
   {
      state.RegisterDefExpression(fieldName, expression);
      return unit;
   }
}