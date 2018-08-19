using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class SendOperatorMessage : Symbol
   {
      Selector selector;
      Precedence precedence;
      Arity arity;

      public SendOperatorMessage(Selector selector, Precedence precedence, Arity arity)
      {
         this.selector = selector;
         this.precedence = precedence;
         this.arity = arity;
      }

      public override void Generate(OperationsBuilder builder) => builder.SendMessage(selector, 1);

      public override Precedence Precedence => precedence;

      public override Arity Arity => arity;

      public override string ToString() => $".{selector.Image}";
   }
}