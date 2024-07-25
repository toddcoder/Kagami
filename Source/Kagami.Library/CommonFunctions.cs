using Kagami.Library.Objects;
using Core.Strings;

namespace Kagami.Library
{
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
   }
}