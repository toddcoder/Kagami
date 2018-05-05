using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;

namespace Kagami.Library.Parsers.Expressions
{
   public class FormatParser : SymbolParser
   {
      public override string Pattern => "^ /(|s|) /('$' ['cdefgnprxsbo'] ('-'? /d+)? ('.' /d+)?)";

      public FormatParser(ExpressionBuilder builder) : base(builder) { }

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var input = tokens[2].Text.Skip(1);
         if (!input.Contains("."))
         {
            if (input.IsMatch("^ ['efgnp']"))
               input += ".6";
            else if (input == "c")
               input += ".2";
         }

         state.Colorize(tokens, Color.Whitespace, Color.Format);

         builder.Add(new StringSymbol(input));

         return Unit.Matched();
      }
   }
}