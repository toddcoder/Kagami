using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class ElseIfParser : ExpressionBlockParser
{
   protected string fieldName;
   protected bool mutable;
   protected bool assignment;

   public ElseIfParser(string fieldName, bool mutable, bool assignment)
   {
      this.fieldName = fieldName;
      this.mutable = mutable;
      this.assignment = assignment;
   }

   //public override string Pattern => "^ /(/s*) /'else' /(/s+) /'if' /b";

   [GeneratedRegex(@"^(\s*)(else)(\s+)(if)\b")]
   public override partial Regex Regex();

   public Maybe<If> If { get; set; } = nil;

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Keyword);
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

      If = new If(expression, block, _elseIf, _elseBlock, fieldName, mutable, assignment, false);
      return unit;
   }
}