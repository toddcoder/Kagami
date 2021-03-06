﻿using System.Linq;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class IndexerParser : SymbolParser
   {
      public IndexerParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /'[' /'+'?";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var insert = tokens[2].Text == "+";
         state.Colorize(tokens, Color.OpenParenthesis, Color.Structure);

         return getArguments(state, builder.Flags).Map(e =>
         {
            if (state.Scan($"^ /(|s|) /({REGEX_ASSIGN_OPS})? /'=' -(> '=')", Color.Whitespace, Color.Operator, Color.Structure)
               .If(out var opSource, out var anyException))
            {
               if (getExpression(state, builder.Flags).ValueOrCast<Unit>(out var expression, out var asUnit))
               {
                  opSource = opSource.DropWhile(" ").Keep(1);
                  var operation = matchOperator(opSource)
                     .FlatMap(o => o.Some(), none<Operations.Operation>, _ => none<Operations.Operation>());
                  if (operation.IsNone && insert)
                  {
                     var list = e.ToList();
                     list.Add(expression);
                     builder.Add(new SendMessageSymbol("insert(at:_<Int>,value:_)", none<LambdaSymbol>(),
                        none<Operations.Operation>(), list.ToArray()));
                  }
                  else
                  {
                     builder.Add(new IndexSetterSymbol(e, expression, operation));
                  }

                  return Unit.Matched();
               }
               else
               {
                  return asUnit;
               }
            }
            else if (anyException.If(out var exception))
            {
               return failedMatch<Unit>(exception);
            }
            else if (e.Length > 0)
            {
               builder.Add(new IndexerSymbol(e));
            }
            else
            {
               return notMatched<Unit>();
            }

            return Unit.Matched();
         });
      }
   }
}