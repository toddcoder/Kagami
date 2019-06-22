using Kagami.Library.Operations;
using Core.Enumerables;

namespace Kagami.Library.Nodes.Symbols
{
   public class PostfixInvokeSymbol : Symbol
   {
      Expression[] arguments;

      public PostfixInvokeSymbol(Expression[] arguments) => this.arguments = arguments;

      public override void Generate(OperationsBuilder builder)
      {
         foreach (var argument in arguments)
         {
	         argument.Generate(builder);
         }

         builder.ToArguments(arguments.Length);
         builder.PostfixInvoke();
      }

      public override Precedence Precedence => Precedence.SendMessage;

      public override Arity Arity => Arity.Postfix;

      public override string ToString() => $"({arguments.Stringify()})";
   }
}