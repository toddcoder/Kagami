using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class SendBinaryMessageSymbol : Symbol
   {
      Selector selector;
      Precedence precedence;
      bool swap;

      public SendBinaryMessageSymbol(Selector selector, Precedence precedence, bool swap=false)
      {
         this.selector = selector;
         this.precedence = precedence;
         this.swap = swap;
      }

      public override void Generate(OperationsBuilder builder)
      {
         if (swap)
            builder.Swap();
         builder.SendMessage(selector, 1);
      }

      public override Precedence Precedence => precedence;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => selector.Image;
   }
}