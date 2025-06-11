using Kagami.Library.Objects;

namespace Kagami.Library.Operations;

public abstract record ReturnValue
{
   public sealed record Value(IObject Object) : ReturnValue;

   public sealed record NoValue : ReturnValue;

   public sealed record EmptyStack : ReturnValue;
}