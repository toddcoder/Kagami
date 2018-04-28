using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library
{
   public static class CommonFunctions
   {
      public static string mangled(object name, object id) => $"__${name}_{id}";

      public static IObject evaluate(IInvokable invokable) => Machine.Current.Invoke(invokable, Arguments.Empty).Value;
   }
}