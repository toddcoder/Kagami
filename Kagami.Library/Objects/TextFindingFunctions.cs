using System;
using System.Linq;
using System.Text;
using Standard.Types.Strings;

namespace Kagami.Library.Objects
{
	public static class TextFindingFunctions
	{
		public static IObject find(string value, string input, int startIndex, bool reverse)
		{
			int index;
			if (reverse)
			{
				if (startIndex == 0)
					index = input.LastIndexOf(value, StringComparison.Ordinal);
				else
					index = input.LastIndexOf(value, startIndex, StringComparison.Ordinal);
			}
			else
				index = input.IndexOf(value, startIndex, StringComparison.Ordinal);

			if (index == -1)
				return Nil.NilValue;
			else
				return Some.Object((Int)index);
      }

		public static Tuple findAll(string value, string input) => new Tuple(input.FindAll(value).Select(Int.IntObject).ToArray());

		public static String replace(string value, string input, string replacement, bool reverse)
		{
			int index;
			if (reverse)
				index = input.LastIndexOf(value, StringComparison.Ordinal);
			else
				index = input.IndexOf(value, StringComparison.Ordinal);

			if (index > -1)
				return input.Take(index) + replacement + input.Skip(index + value.Length);
			else
				return input;
      }

		public static String replace(string value, string input, Lambda lambda, bool reverse)
		{
			int index;
			if (reverse)
				index = input.LastIndexOf(value, StringComparison.Ordinal);
			else
				index = input.IndexOf(value, StringComparison.Ordinal);

			if (index > -1)
			{
				var text = input.Skip(index);
				var length = text.Length;
				var replacement = lambda.Invoke((String)text, (Int)index, (Int)length);

				return input.Take(index) + replacement + input.Skip(index + value.Length);
			}
			else
				return input;
      }

		public static String replaceAll(string value, string input, string replacement) => input.Replace(value, replacement);

		public static String replaceAll(string value, string input, Lambda lambda)
		{
			var builder = new StringBuilder();
			var index = input.IndexOf(value);
			var start = 0;
			while (index > -1)
			{
				var replacement = lambda.Invoke((String)value, (Int)index, (Int)value.Length);
				builder.Append(input.Skip(start));
				builder.Append(replacement.AsString);
				start = index + value.Length;
				index = input.IndexOf(value);
			}

			builder.Append(input.Skip(start));

			return builder.ToString();
      }

		public static Tuple split(string value, string input)
		{
			return new Tuple(input.Split(new[] { value }, StringSplitOptions.None).Select(String.StringObject).ToArray());
      }

		public static Tuple partition(string value, string input, bool reverse)
		{
			if (reverse)
			{
				var index = input.LastIndexOf(value, StringComparison.Ordinal);
				if (index > -1)
					return Tuple.Tuple3(input.Take(index), value, input.Skip(index + value.Length));
				else
					return Tuple.Tuple3(input, "", "");
			}
			else
			{
				if (input.Find(value).If(out var index))
					return Tuple.Tuple3(input.Take(index), value, input.Skip(index + value.Length));
				else
					return Tuple.Tuple3(input, "", "");
			}
      }

		public static Int count(string value, string input) => input.FindAll(value).Count();

		public static Int count(string value, string input, Lambda lambda)
		{
			return input.FindAll(value).Count(i => lambda.Invoke(Int.IntObject(i)).IsTrue);
		}
	}
}