using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public class UserRangeItem : IRangeItem
   {
      UserObject userObject;

      public UserRangeItem(UserObject userObject) => this.userObject = userObject;

      public int Compare(IObject obj)
      {
         var result = sendMessage(userObject, "<>", obj);
         if (result is Int i)
            return i.Value;
         else
            throw incompatibleClasses(result, "Int");
      }

      public IObject Object => userObject;

      public Boolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

      public IRangeItem Successor
      {
         get
         {
            var result = sendMessage(userObject, "succ".get());
            if (result is UserObject uo)
               return new UserRangeItem(uo);
            else
               throw incompatibleClasses(result, userObject.ClassName);
         }
      }

      public IRangeItem Predecessor
      {
         get
         {
            var result = sendMessage(userObject, "pred".get());
            if (result is UserObject uo)
               return new UserRangeItem(uo);
            else
               throw incompatibleClasses(result, userObject.ClassName);
         }
      }

      public Range Range()
      {
         var result = sendMessage(userObject, "range");
         if (result is Range range)
            return range;
         else
            throw incompatibleClasses(result, "Range");
      }
   }
}