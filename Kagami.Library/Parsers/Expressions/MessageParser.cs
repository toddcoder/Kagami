using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class MessageParser : SymbolParser
{
   public MessageParser(ExpressionBuilder builder) : base(builder) { }

   public override string Pattern => $"^ /(|s|) /'&.' /({REGEX_FUNCTION_NAME}) /'('?";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var selector = tokens[3].Text;
      var parameterDelimiter = tokens[4].Text;
      var parseArguments = true;
      if (parameterDelimiter.IsEmpty())
      {
         selector = selector.get();
         parseArguments = false;
      }
      else if (selector.EndsWith("="))
      {
         selector = selector.Drop(-1).set();
         parseArguments = true;
      }

      if (!parseArguments)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Structure, Color.Message);
      }
      else
      {
         state.Colorize(tokens, Color.Whitespace, Color.Structure, Color.Message, Color.OpenParenthesis);
      }

      if (!parseArguments)
      {
         builder.Add(new MessageSymbol(selector, [], nil));
         return unit;
      }
      else
      {
         var _argumentsPlusLambda = getArgumentsPlusLambda(state, builder.Flags);
         if (_argumentsPlusLambda is (true, var (arguments, _lambda)))
         {
            builder.Add(new MessageSymbol(selector, arguments, _lambda));
            return unit;
         }
         else
         {
            return _argumentsPlusLambda.Exception;
         }
      }
   }
}