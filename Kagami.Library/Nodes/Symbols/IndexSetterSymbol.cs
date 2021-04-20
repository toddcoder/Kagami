using Kagami.Library.Operations;
using Core.Enumerables;
using Core.Monads;

namespace Kagami.Library.Nodes.Symbols
{
   public class IndexSetterSymbol : Symbol
   {
      protected Expression[] arguments;
      protected Expression value;
      protected IMaybe<Operation> anyOperation;

      public IndexSetterSymbol(Expression[] arguments, Expression value, IMaybe<Operation> anyOperation)
      {
         this.arguments = arguments;
         this.value = value;
         this.anyOperation = anyOperation;
      }

      public override void Generate(OperationsBuilder builder)
      {
         if (anyOperation.If(out var operation))
         {
            builder.Dup();
            IndexerSymbol.Get(builder, arguments);
            value.Generate(builder);
            builder.AddRaw(operation);
            IndexerSymbol.GetIndex(builder, arguments);
            builder.Swap();
         }
         else
         {
            IndexerSymbol.GetIndex(builder, arguments);
            value.Generate(builder);
         }

         builder.SendMessage("[]=(_)", 2);
      }

      public override Precedence Precedence => Precedence.SendMessage;

      public override Arity Arity => Arity.Postfix;

      public override string ToString() => $"[{arguments.ToString(", ")}] = {value}";
   }
}