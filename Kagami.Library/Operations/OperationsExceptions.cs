using System;
using Kagami.Library.Objects;
using Standard.Types.Exceptions;

namespace Kagami.Library.Operations
{
   public static class OperationsExceptions
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

      public static string messageAddressOutOfRange() => "Address out of range";

      public static Exception addressOutOfRange() => messageAddressOutOfRange().Throws();

      public static string messageFieldNotFound(string name) => $"Field {name} not found";

      public static Exception fieldNotFound(string name) => messageFieldNotFound(name).Throws();

      public static string messageClassNotFound(string name) => $"Class {name} not found";

      public static Exception classNotFound(string name) => messageClassNotFound(name).Throws();

      public static string messageFieldUnassigned(string name) => $"Field {name} unassigned";

      public static Exception fieldUnassigned(string name) => messageFieldUnassigned(name).Throws();

      public static string messageImmutableField(string name) => $"Field {name} is immutable";

      public static Exception immutableField(string name) => messageImmutableField(name).Throws();

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
   }
}