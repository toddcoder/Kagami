using Core.Monads;

namespace Kagami.Library.Operations;

public class And : TwoBooleanOperation
{
   public override Optional<bool> Execute(bool x, bool y) => x && y;

   public override string ToString() => "and";
}