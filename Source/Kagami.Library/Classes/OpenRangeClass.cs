namespace Kagami.Library.Classes
{
   public class OpenRangeClass : BaseClass
   {
      public override string Name => "OpenRange";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         collectionMessages();
      }
   }
}