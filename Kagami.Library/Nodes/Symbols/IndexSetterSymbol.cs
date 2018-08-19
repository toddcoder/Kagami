using Kagami.Library.Operations;
using Standard.Types.Enumerables;
using Standard.Types.Maybe;

namespace Kagami.Library.Nodes.Symbols
{
   public class IndexSetterSymbol : Symbol
   {
      Expression[] arguments;
      Expression value;
      IMaybe<Operation> operation;

      public IndexSetterSymbol(Expression[] arguments, Expression value, IMaybe<Operation> operation)
      {
         this.arguments = arguments;
         this.value = value;
         this.operation = operation;
      }

      public override void Generate(OperationsBuilder builder)
      {
         if (operation.If(out var op))
         {
            builder.Dup();
            builder.SetX();
            builder.SendMessage("[]()", arguments);
            value.Generate(builder);
            builder.AddRaw(op);
            builder.GetX();
            builder.Swap();
            builder.SetX();
         }

         foreach (var argument in arguments)
            argument.Generate(builder);
         if (operation.IsSome)
            builder.GetX();
         else
            value.Generate(builder);
         builder.SendMessage("[]=()", arguments.Length + 1);
      }

      public override Precedence Precedence => Precedence.SendMessage;

      public override Arity Arity => Arity.Postfix;

      public override string ToString() => $"[{arguments.Listify()}] = {value}";
   }
}