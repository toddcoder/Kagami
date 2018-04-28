namespace Kagami.Library.Classes
{
   public class CollectionProxyClass : BaseClass
   {
      public override string Name => "CollectionProxy";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         collectionMessages();
      }
   }
}