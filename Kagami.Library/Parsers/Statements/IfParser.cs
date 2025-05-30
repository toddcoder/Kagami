﻿using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class IfParser : ExpressionBlockParser
{
   protected bool mutable;
   protected string fieldName = "";
   protected bool assignment;

   //public override string Pattern => $"^ (/('var' | 'let') /(/s*) /({REGEX_FIELD}) /(/s*) /'=' /(/s*))? /'if' -(> ['>^']) /b";

   [GeneratedRegex($@"^(\s*)(?:(var|let)(\s*)({REGEX_FIELD})(\s*)(=)(\s*))?(if)(?![>\^])\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      mutable = tokens[2].Text == "var";
      fieldName = tokens[4].Text;
      assignment = fieldName.IsNotEmpty();
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Structure, Color.Whitespace,
         Color.Keyword);

      return unit;
   }

   public override Optional<Unit> Suffix(ParseState state, Expression expression, Block block)
   {
      Maybe<If> _elseIf = nil;
      var elseIfParser = new ElseIfParser(fieldName, mutable, assignment);
      var _scan = elseIfParser.Scan(state);
      if (_scan)
      {
         _elseIf = elseIfParser.If;
      }
      else if (_scan.Exception is (true, var exception))
      {
         return exception;
      }

      Maybe<Block> _elseBlock = nil;
      var elseParser = new ElseParser();
      _scan = elseParser.Scan(state);
      if (_scan)
      {
         _elseBlock = elseParser.Block;
      }
      else if (_scan.Exception is (true, var exception))
      {
         return exception;
      }

      state.AddStatement(new If(expression, block, _elseIf, _elseBlock, fieldName, mutable, assignment, true));
      return unit;
   }
}