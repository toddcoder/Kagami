namespace Kagami.Library.Classes
{
   public class IteratorClass : BaseClass
   {
      public override string Name => "Iterator";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         iteratorMessages();
      }
   }
}