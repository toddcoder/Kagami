using System;
using Kagami.Library.Classes;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Core.Numbers;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library;

public static class AllExceptions
{
   public static string messageIncompatibleClasses(IObject value, string expectedClassName)
   {
      return $"{value.Image} is of class {value.ClassName}; expected {expectedClassName}";
   }

   public static Exception incompatibleClasses(IObject value, string expectedClassName)
   {
      return fail(messageIncompatibleClasses(value, expectedClassName));
   }

   public static string messageBadAddress(int address) => $"Address {address} is invalid";

   public static Exception badAddress(int address) => fail(messageBadAddress(address));

   public static string messageBadIndex(int index) => $"Index {index} is invalid";

   public static Exception badIndex(int index) => fail(messageBadIndex(index));

   public static string messageInvalidIndex(IObject index) => $"Index or key {index} is invalid";

   public static Exception invalidIndex(IObject index) => fail(messageInvalidIndex(index));

   public static string messageAddressOutOfRange() => "Address out of range";

   public static Exception addressOutOfRange() => fail(messageAddressOutOfRange());

   public static string messageFieldNotFound(string name) => $"Field {name} not found";

   public static Exception fieldNotFound(string name) => fail(messageFieldNotFound(name));

   public static string messageFieldAlreadyExists(string name) => $"Field {name} already exists";

   public static Exception fieldAlreadyExists(string name) => fail(messageFieldAlreadyExists(name));

   public static string messageClassNotFound(string name) => $"Class {name} not found";

   public static Exception classNotFound(string name) => fail(messageClassNotFound(name));

   public static string messageMixinNotFound(string name) => $"Mixin {name} not found";

   public static Exception mixinNotFound(string name) => fail(messageMixinNotFound(name));

   public static string messageClassAlreadyExists(string name) => $"Class {name} already exists";

   public static Exception classAlreadyExists(string name) => fail(messageClassAlreadyExists(name));

   public static string messageMixinAlreadyExists(string name) => $"Mixin {name} already exists";

   public static Exception mixinAlreadyExists(string name) => fail(messageMixinAlreadyExists(name));

   public static string messageFieldUnassigned(string name) => $"Field {name} unassigned";

   public static Exception fieldUnassigned(string name) => fail(messageFieldUnassigned(name));

   public static string messageImmutableField(string name) => $"Field {name} is immutable";

   public static Exception immutableField(string name) => fail(messageImmutableField(name));

   public static string messageImmutableValue(string name) => $"{name} object is immutable";

   public static Exception immutableValue(string name) => fail(messageImmutableValue(name));

   public static string messageNotNumeric(string value) => $"{value} isn't numeric";

   public static Exception notNumeric(string value) => fail(messageNotNumeric(value));

   public static string messageRequiresNOperands(int n) => n.Plural("Operation requires # operand(s)");

   public static Exception requiresNOperands(int n) => fail(messageRequiresNOperands(n));

   public static string messageEmptyStack() => "Empty stack";

   public static Exception emptyStack() => fail(messageEmptyStack());

   public static string messageInvalidStack() => "Invalid stack";

   public static Exception invalidStack() => fail(messageInvalidStack());

   public static string messageMessageNotFound(BaseClass cls, string name) => $"{cls.Name} doesn't respond to message {name}";

   public static Exception messageNotFound(BaseClass cls, string name) => fail(messageMessageNotFound(cls, name));

   public static string messageNotNumeric(IObject obj) => $"{obj.Image} isn't numeric";

   public static Exception notNumeric(IObject obj) => fail(messageNotNumeric(obj));

   public static string messageUnableToConvert(string value, string className)
   {
      return $"Unable to convert {value} to object of class {className}";
   }

   public static Exception unableToConvert(string value, string className) => fail(messageUnableToConvert(value, className));

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

   public static string messageConstantRequired(Expression expression) => $"Constant required, found {expression}";

   public static Exception constantRequired(Expression expression) => fail(messageConstantRequired(expression));

   public static string messageCollectionInfinite() => "Collection is infinite";

   public static Exception collectionInfinite() => fail(messageCollectionInfinite());

   public static string messageUnassigned() => "Unassigned";

   public static Exception unassigned() => fail(messageUnassigned());

   public static string messageKeyNotFound(IObject key) => $"Key {key.Image} not found";

   public static Exception keyNotFound(IObject key) => fail(messageKeyNotFound(key));

   public static string messageExceededMaxDepth() => "Exceeded max depth";

   public static Exception exceededMaxDepth() => fail(messageExceededMaxDepth());
}