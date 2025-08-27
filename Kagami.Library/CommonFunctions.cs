using Core.Enumerables;
using Core.Monads;
using Kagami.Library.Objects;
using Core.Strings;
using Kagami.Library.Invokables;

namespace Kagami.Library;

public static class CommonFunctions
{
   public static string mangled(object name, object id) => $"__${name}_{id}";

   public static (BindingType, string name) fromBindingName(string name)
   {
      if (name.StartsWith("+"))
      {
         return (BindingType.Mutable, name.Drop(1));
      }
      else if (name.StartsWith("-"))
      {
         return (BindingType.Immutable, name.Drop(1));
      }
      else
      {
         return (BindingType.Existing, name);
      }
   }

   public static string convertFunctionName(string fromClass, string toClass) => $"__$convert_from_{fromClass}_to_{toClass}";

   public static IObject resultToSuccess<T>(Result<T> _result) where T : IObject
   {
      if (_result is (true, var value))
      {
         return Success.Object(value);
      }
      else
      {
         return Failure.Object(_result.Exception.Message);
      }
   }

   public static string shortenedId(Guid id) => id.ToString().KeepUntil("-");

   public static string placeholderList(Parameters parameters) => placeholderList(parameters.Length);

   public static string placeholderList(int count) => "_".Repeat(count).ToString(",");
}