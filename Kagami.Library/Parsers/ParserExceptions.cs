using System;
using Standard.Types.Exceptions;

namespace Kagami.Library.Parsers
{
   public static class ParserExceptions
   {
      public static string messageUnableToConvert(string value, string className)
      {
         return $"Unable to convert {value} to object of class {className}";
      }

      public static Exception unableToConvert(string value, string className)
      {
         return messageUnableToConvert(value, className).Throws();
      }

      public static string messageOpenString() => "Open string";

      public static Exception openString() => messageOpenString().Throws();

      public static string messageOpenParameters() => "Open parameters";

      public static Exception openParameters() => messageOpenParameters().Throws();

      public static string messageOpenArguments() => "Open arguments";

      public static Exception openArguments() => messageOpenArguments().Throws();

      public static string messageExpectedExpression() => "Expected expression";

      public static Exception expectedExpression() => messageExpectedExpression().Throws();

      public static string messageExpectedValue() => "Expected value";

      public static Exception expectedValue() => messageExpectedValue().Throws();

      public static string messageBadIndentation() => "Bad indentation";

      public static Exception badIndentation() => messageBadIndentation().Throws();

      public static string messageBadHex(string value) => $"{value} is bad hex";

      public static Exception badHex(string value) => messageBadHex(value).Throws();

      public static string messageInvalidSyntax() => "Invalid syntax";

      public static Exception invalidSyntax() => messageInvalidSyntax().Throws();

      public static string messageNeedsOverride(string name) => $"Function {name} needs to be overridden";

      public static Exception needsOverride(string name) => messageNeedsOverride(name).Throws();
   }
}