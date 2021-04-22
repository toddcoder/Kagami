using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class SendPrefixMessage : Symbol
   {
      protected Selector selector;

      public SendPrefixMessage(Selector selector) => this.selector = selector;

      public override void Generate(OperationsBuilder builder) => builder.SendMessage(selector, 0);

      public override Precedence Precedence => Precedence.SendMessage;

      public override Arity Arity => Arity.Postfix;

      public override string ToString() => selector.Image;
   }
}