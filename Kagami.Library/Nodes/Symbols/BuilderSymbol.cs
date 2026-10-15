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

   public BuilderSymbol(Block block, string[] noCaptures, bool capturing)
   {
      var functionName = newLabel("builder");

      List<Parameter> parameters = [];
      if (noCaptures.Length > 0)
      {
         foreach (var noCapture in noCaptures)
         {
            var parameter = new Parameter(false, false, "", noCapture, nil, nil, false, true, false);
            parameters.Add(parameter);
         }
      }

      function = new Function(functionName, new Parameters([.. parameters]), false, block, false, false, "", capturing);
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