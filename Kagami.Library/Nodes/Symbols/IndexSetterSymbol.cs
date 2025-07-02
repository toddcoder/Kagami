using Kagami.Library.Operations;
using Core.Enumerables;
using Core.Monads;
using Kagami.Library.Objects;

namespace Kagami.Library.Nodes.Symbols;

public class IndexSetterSymbol : Symbol, IHasExpressions, IHasExpression
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
         var newSequenceSymbol = new NewSequenceSymbol(arguments);
         newSequenceSymbol.Generate(builder);
         builder.Swap();
      }
      else
      {
         Set(builder, arguments, value);
      }

      Selector selector = "[]=(_,_)";
      builder.SendMessage(selector, 2);
   }

   public override Precedence Precedence => Precedence.SendMessage;

   public override Arity Arity => Arity.Postfix;

   public override string ToString() => $"[{arguments.ToString(", ")}] = {value}";

   public Expression[] Expressions => arguments;

   public Expression Expression => value;
}