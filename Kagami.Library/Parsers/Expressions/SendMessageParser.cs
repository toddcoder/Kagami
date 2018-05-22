using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class SendMessageParser : SymbolParser
   {
      public SendMessageParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => $"^ /(|s|) /['.@'] /({REGEX_FUNCTION_NAME}) /'('?";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var precedence = tokens[2].Text == "." ? Precedence.PostfixOperator : Precedence.ChainedOperator;
         var messageName = tokens[3].Text;
         var parameterDelimiter = tokens[4].Text;
         var parseArguments = true;
         if (parameterDelimiter.IsEmpty())
         {
            messageName = messageName.get();
            parseArguments = false;
         }
         else if (messageName.EndsWith("="))
         {
            messageName = messageName.Skip(-1).set();
            parseArguments = true;
         }

         state.Colorize(tokens, Color.Whitespace, Color.Message, Color.Message, Color.Structure);

         if (!parseArguments)
         {
            builder.Add(new SendMessageSymbol(messageName, precedence));
            return Unit.Matched();
         }
         else if (getArgumentsPlusLambda(state, builder.Flags).If(out var tuple, out var original))
         {
            var (arguments, lambda) = tuple;
            builder.Add(new SendMessageSymbol(messageName, precedence, lambda, arguments));
            return Unit.Matched();
         }
         else
            return original.Unmatched<Unit>();
      }
   }
}