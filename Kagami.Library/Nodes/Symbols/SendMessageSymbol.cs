using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Standard.Types.Enumerables;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Nodes.Symbols
{
   public class SendMessageSymbol : Symbol
   {
      Selector selector;
      Precedence precedence;
      IMaybe<LambdaSymbol> lambda;
      IMaybe<Operation> operation;
      Expression[] arguments;

      public SendMessageSymbol(Selector selector, Precedence precedence, IMaybe<LambdaSymbol> lambda, IMaybe<Operation> operation,
         params Expression[] arguments)
      {
         this.selector = selector;
         this.precedence = precedence;
         this.lambda = lambda;
         this.operation = operation;
         this.arguments = arguments;
      }

      public SendMessageSymbol(Selector selector, Precedence precedence, params Expression[] arguments) :
         this(selector, precedence, none<LambdaSymbol>(), none<Operation>(), arguments) { }

      public SendMessageSymbol(Selector selector, Precedence precedence, IMaybe<Operation> operation, params Expression[] arguments) :
         this(selector, precedence, none<LambdaSymbol>(), operation, arguments) { }

      public SendMessageSymbol(Selector selector, Precedence precedence, IMaybe<LambdaSymbol> lambda, params Expression[] arguments) :
         this(selector, precedence, lambda, none<Operation>(), arguments) { }

      public override void Generate(OperationsBuilder builder)
      {
         if (operation.IsSome)
         {
            builder.Dup();
            var getter = selector.NewName(selector.Name.Skip(-1));
            builder.SendMessage(getter, 0);
         }

	      var index = 0;
         foreach (var argument in arguments)
	         selector.Generate(index++, argument, builder);

	      if (operation.If(out var op))
            builder.AddRaw(op);

         int count;
         if (lambda.If(out var l))
         {
            l.Generate(builder);
            count = arguments.Length + 1;
         }
         else
            count = arguments.Length;

         builder.Peek(Index);
         builder.SendMessage(selector, count);
         builder.NoOp();
      }

      public override Precedence Precedence => precedence;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => $"{(precedence == Precedence.SendMessage ? "." : "@")}{selector.Image}({arguments.Listify()})";
   }
}