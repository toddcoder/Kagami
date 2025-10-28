namespace Kagami.Library;

public class Putter
{
   protected bool putting;
   protected int writeCount;

   public Putter()
   {
      putting = false;
   }

   public int WriteCount
   {
      get => writeCount;
      set => writeCount = value;
   }

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