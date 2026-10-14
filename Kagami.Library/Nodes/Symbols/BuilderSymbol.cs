using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class BuilderSymbol : Symbol
{
   protected Function function;
   protected InvokeSymbol invoke;

   public BuilderSymbol(Block block)
   {
      var functionName = newLabel("builder");
      function = new Function(functionName, Parameters.Empty, false, block, false, false, "", true);
      invoke = new InvokeSymbol(functionName, [], nil, false);
   }

   public override void Generate(OperationsBuilder builder)
   {
      function.Generate(builder);
      invoke.Generate(builder);
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;
}