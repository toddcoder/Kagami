using Kagami.Library.Objects;
using Standard.Types.Strings;

namespace Kagami.Library
{
   public static class CommonFunctions
   {
      public static string mangled(object name, object id) => $"__${name}_{id}";

	   public static (BindingType, string name) fromBindingName(string name)
	   {
		   if (name.StartsWith("+"))
			   return (BindingType.Mutable, name.Skip(1));
			else if (name.StartsWith("-"))
			   return (BindingType.Immutable, name.Skip(1));
		   else
			   return (BindingType.Existing, name);
	   }
   }
}