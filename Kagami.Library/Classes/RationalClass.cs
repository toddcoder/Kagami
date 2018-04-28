namespace Kagami.Library.Classes
{
   public class RationalClass : BaseClass
   {
      public override string Name => "Rational";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         messageNumberMessages();
         compareMessages();
         numericConversionMessages();
      }
   }
}