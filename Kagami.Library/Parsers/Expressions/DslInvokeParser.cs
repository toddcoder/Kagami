using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class DslInvokeParser : SymbolParser
{
   public DslInvokeParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@$"^(\s*)({REGEX_CLASS})(\s*)({{)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var className = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Class, Color.Whitespace, Color.OpenParenthesis);
      var _arguments = getArguments(state, ExpressionFlags.InArgument);
      if (_arguments is (true, var expressions))
      {
         builder.Add(new DslInvokeSymbol(className, expressions));
         return unit;
      }
      else
      {
         return _arguments.Exception;
      }
   }
}