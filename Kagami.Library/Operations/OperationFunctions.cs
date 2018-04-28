using Kagami.Library.Objects;
using Standard.Types.Maybe;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations
{
   public static class OperationFunctions
   {
      public static IResult<IIterator> getIterator(IObject value, bool lazy)
      {
         switch (value)
         {
            case ICollection collection:
               return collection.GetIterator(lazy).Success();
            case IIterator iterator:
               return iterator.Success();
            case Int i:
               return new Range((Int)0, i, false).GetIterator(lazy).Success();
            case UserObject uo:
               var objectCollection = new ObjectCollection(uo);
               return objectCollection.GetIterator(lazy).Success();
            default:
               return $"{value.Image} isn't an iterator nor can it return one".Failure<IIterator>();
         }
      }

      public static IObject sendMessage(IObject obj, string message, params IObject[] arguments)
      {
         return classOf(obj).SendMessage(obj, message, new Arguments(arguments));
      }
   }
}