namespace Kagami.Library;

public class Putter
{
   protected bool putting;

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

   public string Put(string value, string separator)
   {
      if (putting)
      {
         return $"{separator}{value}";
      }

      putting = true;
      return value;
   }

   public void Reset() => putting = false;
}