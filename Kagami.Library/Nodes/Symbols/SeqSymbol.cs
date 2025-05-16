using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class SeqSymbol : Symbol
{
   protected Block block;
   protected string image;

   public SeqSymbol(Block block) => this.block = block;

   public override void Generate(OperationsBuilder builder)
   {
      image = $"seq {block}";
      var functionName = newLabel("seq");
      var function = new Function(functionName, Parameters.Empty, block, true, false, "");
      function.Generate(builder);

      var invokeSymbol = new InvokeSymbol(functionName, [], nil, false);
      invokeSymbol.Generate(builder);

      builder.PushObject(Void.Value);
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => image;
}