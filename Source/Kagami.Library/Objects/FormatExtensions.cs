using System;
using System.Text;
using Core.RegularExpressions;
using Core.Strings;

namespace Kagami.Library.Objects
{
	public static class FormatExtensions
	{
		public static string FormatUsing<T>(this object obj, string format, Func<T, string> func)
		{
			if (obj is DateTime dateTime)
			{
				return dateTime.ToString(format);
			}
			else
			{
				return format.MatchOne("/['cdefgnprxs'] /('-'? /d+)? ('.' /(/d+))?", true).FlatMap(match =>
				{
					var (specifier, width, places) = match.Groups3();

					var result = new StringBuilder("{0");
					if (width.IsNotEmpty())
					{
						result.Append($",{width}");
					}

					if (specifier.IsNotEmpty() && specifier != "s")
					{
						result.Append($":{specifier}");
						if (places.IsNotEmpty())
						{
							result.Append(places);
						}
					}

					result.Append("}");
					return string.Format(result.ToString(), obj);
				}, () => func((T)obj));
			}
		}
	}
}