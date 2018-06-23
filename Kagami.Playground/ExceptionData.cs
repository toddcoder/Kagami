namespace Kagami.Playground
{
   public class ExceptionData
   {
      public ExceptionData(int index, int length)
      {
         Index = index;
         Length = length;
      }

      public int Index { get; }

      public int Length { get; }
   }
}