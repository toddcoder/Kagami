using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class SendBinaryMessageSymbol : Symbol
   {
      string messageName;
      Precedence precedence;
      bool swap;

      public SendBinaryMessageSymbol(string messageName, Precedence precedence, bool swap=false)
      {
         this.messageName = messageName;
         this.precedence = precedence;
         this.swap = swap;
      }

      public override void Generate(OperationsBuilder builder)
      {
         if (swap)
            builder.Swap();
         builder.SendMessage(messageName, 1);
      }

      public override Precedence Precedence => precedence;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => messageName;
   }
}