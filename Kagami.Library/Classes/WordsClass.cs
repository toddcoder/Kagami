namespace Kagami.Library.Classes;

public class WordsClass : BaseClass
{
   public override string Name => "Words";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      collectionMessages();
   }
}