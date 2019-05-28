using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;
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
         var functionName = newLabel("seq");
         var function = new Function(functionName, Parameters.Empty, block, true, false, "");
         function.Generate(builder);

         var invokeSymbol = new InvokeSymbol(functionName, new Expression[0], none<LambdaSymbol>(), false);
         invokeSymbol.Generate(builder);
      }

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => image;
   }
}