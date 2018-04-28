namespace Kagami.Library.Classes
{
   public class Signature
   {
      string fullFunctionName;
      int parameterCount;

      public Signature(string fullFunctionName, int parameterCount)
      {
         this.fullFunctionName = fullFunctionName;
         this.parameterCount = parameterCount;
      }

      public string FullFunctionName => fullFunctionName;

      public int ParameterCount => parameterCount;
   }
}