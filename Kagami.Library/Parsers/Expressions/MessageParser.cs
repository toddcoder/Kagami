﻿using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class MessageParser : SymbolParser
   {
      public MessageParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => $"^ /(|s|) /'&.' /({REGEX_FUNCTION_NAME}) /'('?";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
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
            builder.Add(new MessageSymbol(selector, new Expression[0], none<LambdaSymbol>()));
            return Unit.Matched();
         }
         else
         {
            if (getArgumentsPlusLambda(state, builder.Flags).ValueOrCast<Unit>(out var tuple, out var asUnit))
            {
               var (arguments, lambda) = tuple;
               builder.Add(new MessageSymbol(selector, arguments, lambda));

               return Unit.Matched();
            }
            else
            {
               return asUnit;
            }
         }
      }
   }
}