using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Core.Enumerables;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class SendMessageSymbol : Symbol
{
   protected Selector selector;
   protected Maybe<LambdaSymbol> _lambda;
   protected Maybe<Operation> _operation;
   protected Expression[] arguments;

   public SendMessageSymbol(Selector selector, Maybe<LambdaSymbol> _lambda, Maybe<Operation> _operation,
      params Expression[] arguments)
   {
      this.selector = selector;
      this._lambda = _lambda;
      this._operation = _operation;
      this.arguments = arguments;
   }

   public SendMessageSymbol(Selector selector, params Expression[] arguments) : this(selector, nil, nil, arguments)
   {
   }

   public SendMessageSymbol(Selector selector, Maybe<Operation> _operation, params Expression[] arguments) :
      this(selector, nil, _operation, arguments)
   {
   }

   public SendMessageSymbol(Selector selector, Maybe<LambdaSymbol> _lambda, params Expression[] arguments) :
      this(selector, _lambda, nil, arguments)
   {
   }

   public override void Generate(OperationsBuilder builder)
   {
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

      builder.Peek(Index);
      builder.SendMessage(selector, count);
      builder.NoOp();
   }

   public override Precedence Precedence => Precedence.SendMessage;

   public override Arity Arity => Arity.Postfix;

   public override string ToString() => $".{selector.Image}({arguments.ToString(", ")})";
}