namespace Kagami.Library.Classes;

public class InfinityClass : BaseClass
{
   public override string Name => "Infinity";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      compareMessages();
   }
}