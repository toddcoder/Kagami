using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class IterSymbol : Symbol
{
   protected Block block;
   protected Function function;
   protected InvokeSymbol invoke;

   public IterSymbol(Block block)
   {
      this.block = block;

      var functionName = newLabel("iter");
      function = new Function(functionName, Parameters.Empty, block, true, false, "", true);
      invoke = new InvokeSymbol(functionName, [], nil, false);
   }

   public override void Generate(OperationsBuilder builder)
   {
      function.Generate(builder);
      invoke.Generate(builder);
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"iter {block}";
}