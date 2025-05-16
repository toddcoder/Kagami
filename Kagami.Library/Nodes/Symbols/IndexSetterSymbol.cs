using Kagami.Library.Operations;
using Core.Enumerables;
using Core.Monads;

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

   public override void Generate(OperationsBuilder builder)
   {
      if (_operation is (true, var operation))
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