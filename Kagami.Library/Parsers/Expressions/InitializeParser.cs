using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class InitializeParser : SymbolParser
{
   public InitializeParser(ExpressionBuilder builder) : base(builder)
   {
   }

   //public override string Pattern => $"^ /(/s*) /({REGEX_CLASS}) /'{{'";

   [GeneratedRegex($@"^(\s*)({REGEX_CLASS})({{)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var className = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Class, Color.Structure);

      var list = new List<(string, Expression)>();
      while (state.More)
      {
         var _result =
            from f in state.Scan($@"^(\s*)({REGEX_FIELD})(\s*)(:)", Color.Whitespace, Color.Label, Color.Whitespace, Color.Structure)
            from e in getExpression(state, builder.Flags | ExpressionFlags.OmitComma | ExpressionFlags.OmitColon)
            from n in state.Scan(@"^(\s*)([,\}])", Color.Whitespace, Color.Structure)
            select (field: f.Trim().Drop(-1), expression: e, next: n);
         if (_result is (true, var (field, expression, next)))
         {
            list.Add((field, expression));
            if (next.Trim() == "}")
            {
               builder.Add(new InitializeSymbol(className, list.ToArray()));
               return unit;
            }
         }
         else if (_result.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            return nil;
         }
      }

      return fail("Open initializer");
   }
}