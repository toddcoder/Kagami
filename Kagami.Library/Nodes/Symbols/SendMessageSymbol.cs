using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Core.Enumerables;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class SendMessageSymbol : Symbol
{
   protected Selector selector;
   protected bool optional;
   protected Maybe<LambdaSymbol> _lambda;
   protected Maybe<Operation> _operation;
   protected Expression[] arguments;

   public SendMessageSymbol(Selector selector, bool optional, Maybe<LambdaSymbol> _lambda, Maybe<Operation> _operation,
      params Expression[] arguments)
   {
      this.selector = selector;
      this.optional = optional;
      this._lambda = _lambda;
      this._operation = _operation;
      this.arguments = arguments;
   }

   public SendMessageSymbol(Selector selector, bool optional, params Expression[] arguments) : this(selector, optional, nil, nil, arguments)
   {
   }

   public SendMessageSymbol(Selector selector, bool optional, Maybe<Operation> _operation, params Expression[] arguments) :
      this(selector, optional, nil, _operation, arguments)
   {
   }

   public SendMessageSymbol(Selector selector, bool optional, Maybe<LambdaSymbol> _lambda, params Expression[] arguments) :
      this(selector, optional, _lambda, nil, arguments)
   {
   }

   public override void Generate(OperationsBuilder builder)
   {
      var endLabel = newLabel("end");

      if (optional)
      {
         builder.Dup();
         builder.IsMonad(MonadType.None | MonadType.Failure);
         builder.GoToIfTrue(endLabel);
         builder.SendMessage("value".get(), 0);
      }

      if (_operation)
      {
         builder.Dup();
         var getter = selector.NewName(selector.Name.Drop(-1));
         builder.SendMessage(getter, 0);
      }

      var index = 0;
      foreach (var argument in arguments)
      {
         selector.Generate(index++, argument, builder);
      }

      if (_operation is (true, var operation))
      {
         builder.AddRaw(operation);
      }

      int count;
      if (_lambda is (true, var lambda))
      {
         lambda.Generate(builder);
         count = arguments.Length + 1;
      }
      else
      {
         count = arguments.Length;
      }

      builder.SendMessage(selector, count);

      builder.Label(endLabel);
      builder.NoOp();
   }

   public override Precedence Precedence => Precedence.SendMessage;

   public override Arity Arity => Arity.Postfix;

   public override string ToString() => $".{selector.Image}({arguments.ToString(", ")})";
}