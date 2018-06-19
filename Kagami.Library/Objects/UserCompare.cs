using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public class UserCompare : IObjectCompare
   {
      UserObject userObject;

      public UserCompare(UserObject userObject) => this.userObject = userObject;

      public int Compare(IObject obj)
      {
         var result = sendMessage(userObject, "<>", obj);
         if (result is Int i)
            return i.Value;
         else
            throw incompatibleClasses(result, "Int");
      }

      public IObject Object => userObject;

      public Boolean Between(IObject min, IObject max) => between(this, min, max);
   }
}