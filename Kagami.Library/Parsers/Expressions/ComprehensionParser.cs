﻿using System.Collections.Generic;
using System.Text;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class ComprehensionParser : SymbolParser
   {
      public ComprehensionParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'for' /b";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Keyword);
         if (builder.ToExpression().If(out var expression, out var exception))
         {
            var comprehensions = new List<(Symbol, Expression, IMaybe<Expression>, string)>();

            if (getInnerComprehension(state).If(out var tuple, out var original))
            {
               (var comparisand, var source, var ifExp) = tuple;
               var image = $"for {comparisand} <- {source}";
               comprehensions.Add((comparisand, source, ifExp, image));
            }
            else
               return original.Unmatched<Unit>();

            while (state.More)
            {
               var parser = new InnerComprehensionParser(builder, comprehensions);
               if (parser.Scan(state).If(out _, out var isNotMatched, out var exception1)) { }
               else if (isNotMatched)
                  break;
               else
                  return failedMatch<Unit>(exception1);
            }

            var stack = new Stack<(Symbol, Expression, IMaybe<Expression>, string)>();
            foreach (var item in comprehensions)
               stack.Push(item);

            For2 forStatement;
            var images = new StringBuilder();
            if (stack.Count > 0)
            {
               (var symbol, var source, var ifExp, var image) = stack.Pop();
               images.Append(image);
               var yieldStatement = new Yield(expression);
               var block = new Block(yieldStatement);
               if (ifExp.If(out var boolean))
                  block = new Block(new If(boolean, block));
               forStatement = new For2(symbol, source, block);
            }
            else
               return notMatched<Unit>();

            while (stack.Count > 0)
            {
               (var symbol, var source, var ifExp, var image) = stack.Pop();
               images.Append(image);
               var block = new Block(forStatement);
               if (ifExp.If(out var boolean))
                  block = new Block(new If(boolean, block));
               forStatement = new For2(symbol, source, block);
            }

            builder.Clear();

            var statements = new List<Statement>
            {
               forStatement, new Return(new Expression(new NilSymbol()))
            };

            builder.Add(new ComprehensionSymbol(new Block(statements), images.ToString()));

            return Unit.Matched();
         }
         else
            return failedMatch<Unit>(exception);
      }
   }
}