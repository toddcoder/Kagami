using Kagami.Library.Operations;
using Standard.Types.Enumerables;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Nodes.Symbols
{
   public class SendMessageSymbol : Symbol
   {
      string messageName;
      Precedence precedence;
      IMaybe<LambdaSymbol> lambda;
      IMaybe<Operation> operation;
      Expression[] arguments;

      public SendMessageSymbol(string messageName, Precedence precedence, IMaybe<LambdaSymbol> lambda, IMaybe<Operation> operation,
         params Expression[] arguments)
      {
         this.messageName = messageName;
         this.precedence = precedence;
         this.lambda = lambda;
         this.operation = operation;
         this.arguments = arguments;
      }

      public SendMessageSymbol(string messageName, Precedence precedence, params Expression[] arguments) :
         this(messageName, precedence, none<LambdaSymbol>(), none<Operation>(), arguments) { }

      public SendMessageSymbol(string messageName, Precedence precedence, IMaybe<Operation> operation, params Expression[] arguments) :
         this(messageName, precedence, none<LambdaSymbol>(), operation, arguments) { }

      public SendMessageSymbol(string messageName, Precedence precedence, IMaybe<LambdaSymbol> lambda, params Expression[] arguments) :
         this(messageName, precedence, lambda, none<Operation>(), arguments) { }

      public override void Generate(OperationsBuilder builder)
      {
         if (operation.IsSome)
         {
            builder.Dup();
            var getter = messageName.Skip(-1);
            builder.SendMessage(getter, 0);
         }

         foreach (var argument in arguments)
            argument.Generate(builder);

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
         builder.SendMessage(messageName, count);
         builder.NoOp();
      }

      public override Precedence Precedence => precedence;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => $"{(precedence == Precedence.SendMessage ? "." : "@")}{messageName}({arguments.Listify()})";
   }
}