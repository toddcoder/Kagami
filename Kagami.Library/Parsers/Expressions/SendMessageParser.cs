﻿using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class SendMessageParser : SymbolParser
   {
      public SendMessageParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => $"^ /(/s*) /'.' /({REGEX_FUNCTION_NAME}) /'('?";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var precedence = tokens[2].Text == "." ? Precedence.SendMessage : Precedence.ChainedOperator;
         if (precedence == Precedence.ChainedOperator && builder.Flags[ExpressionFlags.InLambda])
         {
            return notMatched<Unit>();
         }

         var name = tokens[3].Text;
         var parameterDelimiter = tokens[4].Text;
         var parseArguments = true;
         if (parameterDelimiter.IsEmpty())
         {
            name = name.get();
            parseArguments = false;
         }
         else if (name.EndsWith("="))
         {
            name = name.Drop(-1).set();
            parseArguments = true;
         }

         if (parseArguments)
         {
            state.Colorize(tokens, Color.Whitespace, Color.Message, Color.Message, Color.OpenParenthesis);
         }
         else
         {
            state.Colorize(tokens, Color.Whitespace, Color.Message, Color.Message);
         }

         if (!parseArguments)
         {
            Selector selector = name;
            builder.Add(new SendMessageSymbol(selector));
            return Unit.Matched();
         }
         else if (getArgumentsPlusLambda(state, builder.Flags).ValueOrCast<Unit>(out var tuple, out var asUnit))
         {
            var (arguments, lambda) = tuple;
            var selector = name.Selector(arguments.Length);
            builder.Add(new SendMessageSymbol(selector, lambda, arguments));

            return Unit.Matched();
         }
         else
         {
            return asUnit;
         }
      }
   }
}