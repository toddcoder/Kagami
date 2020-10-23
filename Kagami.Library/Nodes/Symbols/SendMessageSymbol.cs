using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Core.Enumerables;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Nodes.Symbols
{
   public class SendMessageSymbol : Symbol
   {
      protected Selector selector;
      protected IMaybe<LambdaSymbol> lambda;
      protected IMaybe<Operation> operation;
      protected Expression[] arguments;

      public SendMessageSymbol(Selector selector, IMaybe<LambdaSymbol> lambda, IMaybe<Operation> operation,
         params Expression[] arguments)
      {
         this.selector = selector;
         this.lambda = lambda;
         this.operation = operation;
         this.arguments = arguments;
      }

      public SendMessageSymbol(Selector selector, params Expression[] arguments) :
         this(selector, none<LambdaSymbol>(), none<Operation>(), arguments) { }

      public SendMessageSymbol(Selector selector, IMaybe<Operation> operation, params Expression[] arguments) :
         this(selector, none<LambdaSymbol>(), operation, arguments) { }

      public SendMessageSymbol(Selector selector, IMaybe<LambdaSymbol> lambda, params Expression[] arguments) :
         this(selector, lambda, none<Operation>(), arguments) { }

      public override void Generate(OperationsBuilder builder)
      {
         if (operation.IsSome)
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

         if (operation.If(out var op))
         {
	         builder.AddRaw(op);
         }

         int count;
         if (lambda.If(out var l))
         {
            l.Generate(builder);
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
}