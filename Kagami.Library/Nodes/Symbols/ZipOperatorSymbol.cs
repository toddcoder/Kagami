using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Parsers.Expressions;

namespace Kagami.Library.Nodes.Symbols
{
   public class ZipOperatorSymbol : Symbol
   {
      Symbol operatorSymbol;

      public ZipOperatorSymbol(Symbol operatorSymbol)
      {
         this.operatorSymbol = operatorSymbol;
      }

      public override void Generate(OperationsBuilder builder)
      {
         var exBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
         exBuilder.Add(new FieldSymbol("0".get()));
         exBuilder.Add(operatorSymbol);
         exBuilder.Add(new FieldSymbol("1".get()));
         if (exBuilder.ToExpression().If(out var expression, out var exception))
         {
            var invokable = new LambdaInvokable(new Parameters(2), $"$0 {operatorSymbol} $1");
            if (builder.RegisterInvokable(invokable, expression, true).IfNot(out _, out exception))
               throw exception;

            var lambda = new Lambda(invokable);
            builder.PushObject(lambda);
            builder.SendMessage("zip".Function("on", "with"), 2);
         }
         else
            throw exception;
      }

      public override Precedence Precedence => Precedence.ChainedOperator;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => $"[{operatorSymbol}]";
   }
}