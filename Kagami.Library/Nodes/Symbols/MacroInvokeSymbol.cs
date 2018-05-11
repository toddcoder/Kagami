using Kagami.Library.Nodes.Statements;
using Kagami.Library.Operations;
using Standard.Types.Enumerables;
using Standard.Types.Maybe;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols
{
   public class MacroInvokeSymbol : Symbol
   {
      protected Function function;
      protected Expression[] arguments;
      protected IMaybe<LambdaSymbol> lambda;

      public MacroInvokeSymbol(Function function, Expression[] arguments, IMaybe<LambdaSymbol> lambda)
      {
         this.function = function;
         this.arguments = arguments;
         this.lambda = lambda;
      }

      public override void Generate(OperationsBuilder builder)
      {
         var block = function.Block;
         if (block.ExpressionStatement(true).If(out var expression))
         {
            builder.BeginMacro(function.Parameters, arguments);
            expression.Generate(builder);
         }
         else
         {
            var label = newLabel("return");
            builder.BeginMacro(function.Parameters, arguments, label);
            block.Generate(builder);

            builder.Label(label);
            builder.NoOp();
         }

         builder.EndMacro();
      }

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => $"{function.FunctionName}({arguments.Listify()})";
   }
}