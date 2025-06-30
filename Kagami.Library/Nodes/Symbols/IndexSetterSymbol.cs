using Core.Arrays;
using Kagami.Library.Operations;
using Core.Enumerables;
using Core.Monads;
using Kagami.Library.Objects;

namespace Kagami.Library.Nodes.Symbols;

public class IndexSetterSymbol : Symbol
{
   protected Expression[] arguments;
   protected Expression value;
   protected Maybe<Operation> _operation;

   public IndexSetterSymbol(Expression[] arguments, Expression value, Maybe<Operation> _operation)
   {
      this.arguments = arguments;
      this.value = value;
      this._operation = _operation;
   }

   public static void Set(OperationsBuilder builder, Expression[] arguments, Expression value)
   {
      foreach (var expression in arguments)
      {
         expression.Generate(builder);
      }

      value.Generate(builder);
   }

   public override void Generate(OperationsBuilder builder)
   {
      if (_operation is (true, var operation))
      {
         builder.Dup();
         IndexerSymbol.Get(builder, arguments);
         value.Generate(builder);
         builder.AddRaw(operation);
         foreach (var expression in arguments)
         {
            expression.Generate(builder);
         }
         builder.Swap();
      }
      else
      {
         Set(builder, arguments, value);
      }

      var length = arguments.Length + 1;
      Selector selector = $"[]=({length.Repeat("_")})";
      builder.SendMessage(selector, length);
   }

   public override Precedence Precedence => Precedence.SendMessage;

   public override Arity Arity => Arity.Postfix;

   public override string ToString() => $"[{arguments.ToString(", ")}] = {value}";
}