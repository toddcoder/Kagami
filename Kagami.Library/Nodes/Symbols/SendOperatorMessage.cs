using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class SendOperatorMessage : Symbol
   {
      string messageName;
      Precedence precedence;
      Arity arity;

      public SendOperatorMessage(string messageName, Precedence precedence, Arity arity)
      {
         this.messageName = messageName;
         this.precedence = precedence;
         this.arity = arity;
      }

      public override void Generate(OperationsBuilder builder) => builder.SendMessage(messageName, 1);

      public override Precedence Precedence => precedence;

      public override Arity Arity => arity;

      public override string ToString() => $".{messageName}";
   }
}