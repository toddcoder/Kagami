using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class PipeToFieldSymbol(string fieldName) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.NewFieldTolerant(fieldName, true, true);
      builder.AssignField(fieldName, true);
   }

   public override Precedence Precedence => Precedence.Pipeline;

   public override Arity Arity => Arity.Nullary;
}