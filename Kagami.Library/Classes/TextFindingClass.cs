using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Classes;

public class TextFindingClass : BaseClass
{
   public override string Name => "TextFinding";

   public override bool MatchCompatible(BaseClass otherClass)
   {
      return otherClass is StringClass || otherClass is CharClass || otherClass is RegexClass;
   }

   public override bool AssignCompatible(BaseClass otherClass) => MatchCompatible(otherClass);

   public override IObject DefaultValue => throw noDefaultValue("TextFinding");
}