using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class JunctionParser : SymbolParser
{
   public JunctionParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(all|any|one|none)(\[)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var type = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.OpenParenthesis);

      var _expressions = getExpressions(state, @"^(\s*)(\])");
      if (_expressions is (true, var expressions))
      {
         /*if (expressions.Length <= 1)
         {
            return fail("Junctions must have at least 2 items");
         }*/

         builder.Add(new JunctionSymbol(type, expressions));
         return unit;
      }
      else
      {
         return _expressions.Exception;
      }
   }
}