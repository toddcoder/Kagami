namespace Kagami.Library.Objects;

public interface IIncrementDecrement
{
   IObject Increment(int amount = 1);

   IObject Decrement(int amount = 1);
}