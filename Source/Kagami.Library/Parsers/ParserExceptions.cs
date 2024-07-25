using System;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers;

public static class ParserExceptions
{
   public static string messageUnableToConvert(string value, string className)
   {
      return $"Unable to convert {value} to object of class {className}";
   }

   public static Exception unableToConvert(string value, string className)
   {
      return fail(messageUnableToConvert(value, className));
   }

   public static string messageOpenString() => "Open string";

   public static Exception openString() => fail(messageOpenString());

   public static string messageOpenParameters() => "Open parameters";

   public static Exception openParameters() => fail(messageOpenParameters());

   public static string messageOpenArguments() => "Open arguments";

   public static Exception openArguments() => fail(messageOpenArguments());

   public static string messageExpectedExpression() => "Expected expression";

   public static Exception expectedExpression() => fail(messageExpectedExpression());

   public static string messageExpectedValue() => "Expected value";

   public static Exception expectedValue() => fail(messageExpectedValue());

   public static string messageBadIndentation() => "Bad indentation";

   public static Exception badIndentation() => fail(messageBadIndentation());

   public static string messageBadHex(string value) => $"{value} is bad hex";

   public static Exception badHex(string value) => fail(messageBadHex(value));

   public static string messageInvalidSyntax() => "Invalid syntax";

   public static Exception invalidSyntax() => fail(messageInvalidSyntax());

   public static string messageNeedsOverride(string name) => $"Function {name} needs to be overridden";

   public static Exception needsOverride(string name) => fail(messageNeedsOverride(name));
}