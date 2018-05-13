namespace Kagami.Library.Parsers.Expressions
{
   public enum RegexParsingType
   {
      Outside,
      WaitingForSingleQuote,
      WaitingForDoubleQuote,
      EscapedSingleQuote,
      EscapedDoubleQuote,
      AwaitingOption
   }
}