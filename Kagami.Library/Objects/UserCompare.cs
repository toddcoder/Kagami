﻿using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public class UserCompare : IObjectCompare
   {
      protected UserObject userObject;

      public UserCompare(UserObject userObject) => this.userObject = userObject;

      public int Compare(IObject obj)
      {
         var result = sendMessage(userObject, "<>", obj);
         if (result is Int i)
         {
            return i.Value;
         }
         else
         {
            throw incompatibleClasses(result, "Int");
         }
      }

      public IObject Object => userObject;

      public KBoolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

      public KBoolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);
   }
}