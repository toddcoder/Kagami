using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public class WordsClass : BaseClass
{
   public override string Name => "Words";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      collectionMessages();
   }

   public override IObject DefaultValue => new Words((KString)KString.Empty);
}