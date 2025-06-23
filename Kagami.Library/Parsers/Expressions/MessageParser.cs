using System.Text.RegularExpressions;
using Core.Enumerables;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Kagami.Library.Objects;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Expressions;

public partial class MessageParser : SymbolParser
{
   public MessageParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@$"^(\s*)(&\.)({REGEX_FUNCTION_NAME})(\()?")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var selectorSource = tokens[3].Text;
      var hasDelimiter = tokens[4].Text == "(";
      /*var parseArguments = true;
      if (parameterDelimiter.IsEmpty())
      {
         selector = selector.get();
         parseArguments = false;
      }
      else if (selector.EndsWith("="))
      {
         selector = selector.Drop(-1).set();
         parseArguments = true;
      }*/

      if (!hasDelimiter)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Message, Color.Message);
      }
      else
      {
         state.Colorize(tokens, Color.Whitespace, Color.Message, Color.Message, Color.OpenParenthesis);
      }

      if (!hasDelimiter)
      {
         Selector selector = selectorSource;
         builder.Add(new MessageSymbol(selector, [], nil));
         return unit;
      }
      else
      {
         var _argumentsPlusLambda = getArgumentsPlusLambda(state, builder.Flags);
         if (_argumentsPlusLambda is (true, var (arguments, _lambda)))
         {
            Selector selector = $"{selectorSource}({Enumerable.Range(0, arguments.Length).Select(_ => "_").ToString(",")})";
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