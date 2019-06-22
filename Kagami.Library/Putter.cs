namespace Kagami.Library
{
   public class Putter
   {
      bool putting;

      public Putter() => putting = false;

      public string Put(string value)
      {
         if (putting)
         {
	         return $" {value}";
         }

         putting = true;
         return value;
      }

      public void Reset() => putting = false;
   }
}