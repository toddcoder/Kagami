using System;

namespace Kagami.Library.Classes
{
	[Obsolete("Replace with selector")]
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