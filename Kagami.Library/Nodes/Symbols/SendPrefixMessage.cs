using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class SendPrefixMessage : Symbol
   {
      string messageName;

      public SendPrefixMessage(string messageName) => this.messageName = messageName;

      public override void Generate(OperationsBuilder builder) => builder.SendMessage(messageName, 0);

      public override Precedence Precedence => Precedence.SendMessage;

      public override Arity Arity => Arity.Postfix;

      public override string ToString() => messageName;
   }
}