using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols
{
   public class SeqSymbol : Symbol
   {
      Block block;
      string image;

      public SeqSymbol(Block block) => this.block = block;

      public override void Generate(OperationsBuilder builder)
      {
         image = $"seq {block}";
         var invokable = new YieldingInvokable(newLabel("seq"), Parameters.Empty, image);
         if (builder.RegisterInvokable(invokable, block, false).If(out _, out var exception))
         {
            var lambda = new Lambda(invokable);
            builder.PushObject(lambda);
         }
         else
            throw exception;
      }

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => image;
   }
}