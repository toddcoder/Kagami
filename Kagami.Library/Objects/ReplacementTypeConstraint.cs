using Kagami.Library.Classes;

namespace Kagami.Library.Objects;

public class ReplacementTypeConstraint(BaseClass baseClass) : TypeConstraint([baseClass])
{
   public void Replace(BaseClass newBaseClass)
   {
      comparisands[0] = newBaseClass;
   }

   public override string AsString => comparisands[0].Name;
}