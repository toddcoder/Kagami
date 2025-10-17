using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class LazyBlockSymbol : Symbol
{
   protected string functionName = newLabel("lazy");
   protected Function function;

   public LazyBlockSymbol(Block block)
   {
      function = new Function(functionName, Parameters.Empty, block, false, false, "") { IsFixed = true };
   }

   public override void Generate(OperationsBuilder builder)
   {
      function.Generate(builder);
      var invokable = function.Invokable;
      builder.PushObject(new Lazy(invokable, invokable.Image));
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"lazy {{{function.Block}}}";
}