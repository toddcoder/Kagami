namespace Kagami.Library.Parsers.Expressions;

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
   OmitNameValue = 8192,
   OmitIn = 16384,
   OmitBind = 32768,
   OmitAssign = 65536,
   OmitMaybe = 131072,
   OmitNot = 262144,
   OmitLambda = 524288,
   InSubExpression = 1048576,
   OmitWhileUntil = 2097152
}