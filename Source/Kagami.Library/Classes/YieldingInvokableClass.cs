namespace Kagami.Library.Classes
{
   public class YieldingInvokableClass : BaseClass
   {
      public override string Name => "YieldingInvokable";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         //iteratorMessages();
         collectionMessages();
      }
   }
}