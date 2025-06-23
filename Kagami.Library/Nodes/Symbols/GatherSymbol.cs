using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class GatherSymbol : Symbol
{
   protected Block block;
   protected Function function;
   protected InvokeSymbol invoke;

   public GatherSymbol(Block block)
   {
      this.block = block;

      var functionName = newLabel("gather");
      function = new Function(functionName, Parameters.Empty, block, true, false, "");
      invoke = new InvokeSymbol(functionName, [], nil, false);
   }

   public override void Generate(OperationsBuilder builder)
   {
      function.Generate(builder);
      invoke.Generate(builder);
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"gather {block}";
}