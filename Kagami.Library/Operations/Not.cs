using Core.Monads;

namespace Kagami.Library.Operations;

public class Not : OneBooleanOperation
{
   public override Optional<bool> Execute(bool boolean) => !boolean;

   public override string ToString() => "not";
}