using System;
using System.Linq;
using System.Text;
using Core.Strings;

namespace Kagami.Library.Objects
{
	public static class TextFindingFunctions
	{
		public static IObject find(string value, string input, int startIndex, bool reverse)
		{
			int index;
			if (reverse)
         {
            index = startIndex == 0 ? input.LastIndexOf(value, StringComparison.Ordinal) : input.LastIndexOf(value, startIndex, StringComparison.Ordinal);
         }
			else
			{
				index = input.IndexOf(value, startIndex, StringComparison.Ordinal);
			}

			return index == -1 ? None.NoneValue : Some.Object((Int)index);
		}

		public static Tuple findAll(string value, string input) => new(input.FindAll(value).Select(Int.IntObject).ToArray());

		public static String replace(string value, string input, string replacement, bool reverse)
      {
         var index = reverse ? input.LastIndexOf(value, StringComparison.Ordinal) : input.IndexOf(value, StringComparison.Ordinal);

         if (index > -1)
			{
				return input.Keep(index) + replacement + input.Drop(index + value.Length);
			}
			else
			{
				return input;
			}
      }

		public static String replace(string value, string input, Lambda lambda, bool reverse)
      {
         var index = reverse ? input.LastIndexOf(value, StringComparison.Ordinal) : input.IndexOf(value, StringComparison.Ordinal);

         if (index > -1)
			{
				var text = input.Drop(index);
				var length = text.Length;
				var replacement = lambda.Invoke((String)text, (Int)index, (Int)length);

				return input.Keep(index) + replacement + input.Drop(index + value.Length);
			}
			else
			{
				return input;
			}
      }

		public static String replaceAll(string value, string input, string replacement) => input.Replace(value, replacement);

		public static String replaceAll(string value, string input, Lambda lambda)
		{
			var builder = new StringBuilder();
			var index = input.IndexOf(value, StringComparison.Ordinal);
			var start = 0;
			while (index > -1)
			{
				var replacement = lambda.Invoke((String)value, (Int)index, (Int)value.Length);
				builder.Append(input.Drop(start));
				builder.Append(replacement.AsString);
				start = index + value.Length;
				index = input.IndexOf(value, StringComparison.Ordinal);
			}

			builder.Append(input.Drop(start));

			return builder.ToString();
      }

		public static Tuple split(string value, string input)
		{
			return new(input.Split(new[] { value }, StringSplitOptions.None).Select(String.StringObject).ToArray());
      }

		public static Tuple partition(string value, string input, bool reverse)
		{
			if (reverse)
			{
				var index = input.LastIndexOf(value, StringComparison.Ordinal);
				if (index > -1)
				{
					return Tuple.Tuple3(input.Keep(index), value, input.Drop(index + value.Length));
				}
				else
				{
					return Tuple.Tuple3(input, "", "");
				}
			}
			else
			{
				if (input.Find(value).If(out var index))
				{
					return Tuple.Tuple3(input.Keep(index), value, input.Drop(index + value.Length));
				}
				else
				{
					return Tuple.Tuple3(input, "", "");
				}
			}
      }

		public static Int count(string value, string input) => input.FindAll(value).Count();

		public static Int count(string value, string input, Lambda lambda)
		{
			return input.FindAll(value).Count(i => lambda.Invoke(Int.IntObject(i)).IsTrue);
		}
	}
}