using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class AttemptSymbol(Symbol symbol) : Symbol, IHasSymbol
{
   public override void Generate(OperationsBuilder builder)
   {
      var tryLabel = newLabel("try");
      var errorLabel = newLabel("error");
      var endLabel = newLabel("end");

      builder.PushFunctionFrame();
      builder.TryBegin(tryLabel);
      builder.SetErrorHandler(errorLabel);

      symbol.Generate(builder);

      builder.Label(tryLabel);
      builder.PopTryFrame();
      builder.GoTo(endLabel);

      builder.Label(errorLabel);
      builder.NewFailure();

      builder.Label(endLabel);
      builder.Return(true);
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"attempt({symbol})";

   public Symbol Symbol => symbol;
}