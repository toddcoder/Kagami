using Kagami.Library.Operations;
using Core.Enumerables;
using Core.Monads;
using Kagami.Library.Objects;
using static Kagami.Library.CommonFunctions;
using static Kagami.Library.Nodes.NodeFunctions;

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
      var isUserClassLabel = newLabel("is-user-class");
      var endLabel = newLabel("end");
      builder.Dup();
      builder.IsUserClass();
      builder.GoToIfTrue(isUserClassLabel);

      if (_operation is (true, var operation1))
      {
         builder.Dup();
         IndexerSymbol.Get(builder, arguments);
         value.Generate(builder);
         builder.AddRaw(operation1);
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
      builder.GoTo(endLabel);

      builder.Label(isUserClassLabel);

      if (_operation is (true, var operation2))
      {
         builder.Dup();
         selector = $"[]({placeholderList(arguments.Length)})";
         builder.SendMessage(selector, arguments);
         value.Generate(builder);
         builder.AddRaw(operation2);
         builder.SetX();
         foreach (var argument in arguments)
         {
            argument.Generate(builder);
         }
         builder.GetX();

         selector = $"[]=({placeholderList(arguments.Length + 1)})";
         builder.SendMessage(selector, arguments.Length + 1);
      }
      else
      {
         selector = $"[]=({placeholderList(arguments.Length + 1)})";
         builder.SendMessage(selector, [.. arguments, value]);
      }

      builder.Label(endLabel);
      builder.NoOp();
   }

   public override Precedence Precedence => Precedence.SendMessage;

   public override Arity Arity => Arity.Postfix;

   public override string ToString() => $"[{arguments.ToString(", ")}] = {value}";

   public Expression[] Expressions => arguments;

   public Expression Expression => value;
}