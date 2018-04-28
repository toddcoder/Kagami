using Kagami.Library.Operations;
using Standard.Types.Enumerables;
using Standard.Types.Maybe;

namespace Kagami.Library.Nodes.Symbols
{
   public class InvokeSymbol : Symbol
   {
      protected string functionName;
      protected Expression[] arguments;
      protected IMaybe<LambdaSymbol> lambda;

      public InvokeSymbol(string functionName, Expression[] arguments, IMaybe<LambdaSymbol> lambda)
      {
         this.functionName = functionName;
         this.arguments = arguments;
         this.lambda = lambda;
      }

      public override void Generate(OperationsBuilder builder)
      {
         foreach (var argument in arguments)
            argument.Generate(builder);
         int count;
         if (lambda.If(out var l))
         {
            l.Generate(builder);
            count = arguments.Length + 1;
         }
         else
            count = arguments.Length;

         builder.Invoke(functionName, count);
      }

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => $"{functionName}({arguments.Listify()})";
   }
}