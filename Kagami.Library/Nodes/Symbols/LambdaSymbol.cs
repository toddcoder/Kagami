using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class LambdaSymbol : Symbol
   {
      Parameters parameters;
      Block block;

      public LambdaSymbol(Parameters parameters, Block block)
      {
         this.parameters = parameters;
         this.block = block;
      }

      public LambdaSymbol(int unknownFieldCount, Expression expression)
      {
         parameters = new Parameters(unknownFieldCount);
         block = (Block)expression;
      }

      public LambdaSymbol(int unknownFieldCount, Block block)
      {
         parameters = new Parameters(unknownFieldCount);
         this.block = block;
      }

      public override void Generate(OperationsBuilder builder)
      {
         var invokable = new LambdaInvokable(parameters, ToString());
         if (builder.RegisterInvokable(invokable, block, true).If(out _, out var exception))
         {
            builder.NewLambda(invokable);
            builder.Peek(Index);
         }
         else
            throw exception;
      }

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => $"({parameters}) -> {block}";
   }
}