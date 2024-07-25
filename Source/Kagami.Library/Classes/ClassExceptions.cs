using System;
using Kagami.Library.Objects;
using Standard.Types.Exceptions;

namespace Kagami.Library.Classes
{
   public static class ClassExceptions
   {
      public static string messageMessageNotFound(BaseClass cls, string name) => $"{cls.Name} doesn't respond to message {name}";

      public static Exception messageNotFound(BaseClass cls, string name) => messageMessageNotFound(cls, name).Throws();

      public static string messageNotNumeric(IObject obj) => $"{obj.Image} isn't numeric";

      public static Exception notNumeric(IObject obj) => messageNotNumeric(obj).Throws();
   }
}