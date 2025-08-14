using Core.Monads;
using Core.Monads.Lazy;
using Core.Strings;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using System.Text.RegularExpressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Expressions;

public partial class PendingSendMessageParser : SymbolParser
{
   public PendingSendMessageParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex($@"^(\s*)(%)(\.)({REGEX_FUNCTION_NAME})(\()?")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var name = tokens[4].Text;
      var parameterDelimiter = tokens[5].Text;
      var parseArguments = true;

      if (parameterDelimiter.IsEmpty())
      {
         name = name.get();
         parseArguments = false;
      }
      else if (name.EndsWith("="))
      {
         if (builder.Flags[ExpressionFlags.OmitAssign])
         {
            return nil;
         }

         name = name.Drop(-1).set();
         parseArguments = true;
      }

      if (parseArguments)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Message, Color.Message, Color.OpenParenthesis);
      }
      else
      {
         state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Message, Color.Message);
      }

      LazyOptional<(Expression[], Maybe<LambdaSymbol>)> _argumentsPlusLambda = nil;
      if (!parseArguments)
      {
         Selector selector = name;
         var sendMessageSymbol = new SendMessageSymbol(selector, Precedence.SendMessage, false);
         builder.Add(new PendingSendMessage(sendMessageSymbol));
      }
      else if (_argumentsPlusLambda.ValueOf(getArgumentsPlusLambda(state, builder.Flags)) is (true, var (arguments, _lambda)))
      {
         var selector = name.Selector(arguments.Length);
         var sendMessageSymbol = new SendMessageSymbol(selector, Precedence.SendMessage, false, _lambda, arguments);
         builder.Add(new PendingSendMessage(sendMessageSymbol));
      }
      else
      {
         return _argumentsPlusLambda.Exception;
      }

      return unit;
   }
}