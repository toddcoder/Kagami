using System;

namespace Kagami.Library.Parsers.Expressions
{
   [Flags]
   public enum ExpressionFlags
   {
      Standard = 0,
      Comparisand = 1,
      OmitComma = 2,
      OmitColon = 4,
      Subset = 8,
      OmitRange = 16,
      OmitIf = 32,
      OmitComprehension = 64,
      OmitAnd = 128,
      OmitConjunction = 256,
      OmitSendMessageAssign = 512,
      OmitConcatenate = 1024,
		InLambda = 2048,
		InArgument = 4096,
		OmitNameValue = 2 * 4096
   }
}