using System;
using Kagami.Library.Classes;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Standard.Types.Exceptions;

namespace Kagami.Library
{
   public static class AllExceptions
   {
      public static string messageIncompatibleClasses(IObject value, string expectedClassName)
      {
         return $"{value.Image} is of class {value.ClassName}; expected {expectedClassName}";
      }

      public static Exception incompatibleClasses(IObject value, string expectedClassName)
      {
         return messageIncompatibleClasses(value, expectedClassName).Throws();
      }

      public static string messageBadAddress(int address) => $"Address {address} is invalid";

      public static Exception badAddress(int address) => messageBadAddress(address).Throws();

      public static string messageBadIndex(int index) => $"Index {index} is invalid";

      public static Exception badIndex(int index) => messageBadIndex(index).Throws();

      public static string messageAddressOutOfRange() => "Address out of range";

      public static Exception addressOutOfRange() => messageAddressOutOfRange().Throws();

      public static string messageFieldNotFound(string name) => $"Field {name} not found";

      public static Exception fieldNotFound(string name) => messageFieldNotFound(name).Throws();

      public static string messageFieldAlreadyExists(string name) => $"Field {name} already exists";

      public static Exception fieldAlreadyExists(string name) => messageFieldAlreadyExists(name).Throws();

      public static string messageClassNotFound(string name) => $"Class {name} not found";

      public static Exception classNotFound(string name) => messageClassNotFound(name).Throws();

      public static string messageTraitNotFound(string name) => $"Trait {name} not found";

      public static Exception traitNotFound(string name) => messageTraitNotFound(name).Throws();

      public static string messageClassAlreadyExists(string name) => $"Class {name} already exists";

      public static Exception classAlreadyExists(string name) => messageClassAlreadyExists(name).Throws();

      public static string messageTraitAlreadyExists(string name) => $"Trait {name} already exists";

      public static Exception traitAlreadyExists(string name) => messageTraitAlreadyExists(name).Throws();

      public static string messageFieldUnassigned(string name) => $"Field {name} unassigned";

      public static Exception fieldUnassigned(string name) => messageFieldUnassigned(name).Throws();

      public static string messageImmutableField(string name) => $"Field {name} is immutable";

      public static Exception immutableField(string name) => messageImmutableField(name).Throws();

      public static string messageImmutableValue(string name) => $"A {name} object is immutable";

      public static Exception immutableValue(string name) => messageImmutableValue(name).Throws();

      public static string messageNotNumeric(string value) => $"{value} isn't numeric";

      public static Exception notNumeric(string value) => messageNotNumeric(value).Throws();

      public static string messageRequiresNOperands(int n)
      {
         return n == 1 ? "Operation requires 1 operand" : $"Operation requires {n} operands";
      }

      public static Exception requiresNOperands(int n) => messageRequiresNOperands(n).Throws();

      public static string messageEmptyStack() => "Empty stack";

      public static Exception emptyStack() => messageEmptyStack().Throws();

      public static string messageInvalidStack() => "Invalid stack";

      public static Exception invalidStack() => messageInvalidStack().Throws();

      public static string messageMessageNotFound(BaseClass cls, string name) => $"{cls.Name} doesn't respond to message {name}";

      public static Exception messageNotFound(BaseClass cls, string name) => messageMessageNotFound(cls, name).Throws();

      public static string messageNotNumeric(IObject obj) => $"{obj.Image} isn't numeric";

      public static Exception notNumeric(IObject obj) => messageNotNumeric(obj).Throws();

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

      public static string messageConstantRequired(Expression expression) => $"Constant required, found {expression}";

      public static Exception constantRequired(Expression expression) => messageConstantRequired(expression).Throws();

      public static string messageCollectionInfinite() => "Collection is infinite";

      public static Exception collectionInfinite() => messageCollectionInfinite().Throws();

      public static string messageUnassignedd() => "Unassigned";

      public static Exception unassigned() => messageUnassignedd().Throws();
   }
}