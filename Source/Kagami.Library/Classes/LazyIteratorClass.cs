namespace Kagami.Library.Classes
{
   public class LazyIteratorClass : BaseClass
   {
      public override string Name => "LazyIterator";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         iteratorMessages();
      }
   }
}