using Kagami.Library.Invokables;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class Defer : Statement
{
   protected Block block;

   public Defer(Block block)
   {
      this.block = block;
   }

   public override void Generate(OperationsBuilder builder)
   {
      var invokable = new LambdaInvokable(Parameters.Empty, block.ToString());
      var _index = builder.RegisterInvokable(invokable, block, true);
      if (_index)
      {
         builder.NewLambda(invokable, true);
         builder.Defer();
      }
      else
      {
         throw _index.Exception;
      }
   }

   public override string ToString() => $"defer {block}";
}