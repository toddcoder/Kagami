using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public class SequenceClass : BaseClass
{
   public override string Name => "Sequence";

   public override IObject DefaultValue => new Sequence([]);
}