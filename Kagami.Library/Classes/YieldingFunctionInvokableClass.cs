namespace Kagami.Library.Classes
{
   public class YieldingFunctionInvokableClass : BaseClass
   {
      public override string Name => "YieldingFunctionInvokable";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         iteratorMessages();
      }
   }
}