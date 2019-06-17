using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kagami.Library.Classes;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Enumerables;
using Core.Exceptions;
using Core.Monads;
using Core.RegularExpressions;
using Core.Strings;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects
{
	public static class ObjectFunctions
	{
		const int BREAK_EARLY = 10;

		static int uniqueID;

		public static void resetUniqueID() => uniqueID = 0;

		public static int uniqueObjectID() => uniqueID++;

		public static BaseClass classOf(IObject value)
		{
			var className = value.ClassName;
			return Module.Global.Class(className).Required(messageClassNotFound(className));
		}

		public static bool match<T>(T source, IObject comparisand, Func<T, T, bool> equalifier, Hash<string, IObject> bindings)
			where T : IObject
		{
			switch (comparisand)
			{
				case Binding binding:
					comparisand = binding.Value;
					bindings[binding.Name] = source;
					break;
				case NameValue nameValue:
					comparisand = nameValue.Value;
					bindings[$"-{nameValue.Name}"] = source;
					break;
			}

			switch (comparisand)
			{
				case Any _:
					return true;
				case Class cls:
					return classOf(source).MatchCompatible(classOf(cls));
				case Placeholder ph:
					bindings[ph.Name] = source;
					return true;
				case Range range:
					return range.In(source).IsTrue;
				case Lambda lambda:
					if (lambda.Invoke(source).IsTrue)
					{
						var bindingName = $"-{lambda.Invokable.Parameters[0].Name}";
						bindings[bindingName] = source;
						return true;
					}
					else
						return false;
				case InternalList internalList:
					return internalList.In(source);
				case Regex regex:
					return regex.IsMatch(source.AsString).IsTrue;
				case Pattern pattern:
					return pattern.Match(source, bindings);
				case IProcessPlaceholders _:
					return processPlaceholdersMatch(source, comparisand, bindings);
				case TypeConstraint typeConstraint:
					return typeConstraint.Matches(classOf(source));
				default:
					if (classOf(source).MatchCompatible(classOf(comparisand)))
						return equalifier(source, (T)comparisand);
					else
						return false;
			}
		}

		public static bool match<T>(T source, IObject comparisand, Hash<string, IObject> bindings)
			where T : IObject
		{
			return match(source, comparisand, (x, y) => x.IsEqualTo(y), bindings);
		}

/*		static bool matchOnUserObject(UserObject userObject, IObject comparisand, Hash<string, IObject> bindings)
		{
			var userClass = (UserClass)classOf(userObject);
			var parameters = userObject.Parameters.Select(p => "_").Stringify(",");
			var extractMessage = $"extract({parameters})";

			if (userClass.RespondsTo(extractMessage))
			{
					 var result = sendMessage(userObject,extractMessage)
			}
		}*/

		public static bool matchSingle<T>(T source, IObject comparisand, Func<T, IObject, bool> equalifier,
			Hash<string, IObject> bindings)
			where T : IObject
		{
			switch (comparisand)
			{
				case Binding binding:
					comparisand = binding.Value;
					bindings[binding.Name] = source;
					break;
				case NameValue nameValue:
					comparisand = nameValue.Value;
					bindings[$"-{nameValue.Name}"] = source;
               break;
			}

			switch (comparisand)
			{
				case Any _:
					return true;
				case Class _:
					return true;
				case Placeholder ph:
					bindings[ph.Name] = source;
					return true;
				case Range range:
					return range.In(source).IsTrue;
				case Lambda lambda:
					if (lambda.Invoke(source).IsTrue)
					{
						var bindingName = $"-{lambda.Invokable.Parameters[0].Name}";
						bindings[bindingName] = source;
						return true;
               }
					else
						return false;
				case Pattern pattern:
					return pattern.Match(source, bindings);
				case IProcessPlaceholders _:
					return processPlaceholdersMatch(source, comparisand, bindings);
				case TypeConstraint typeConstraint:
					return typeConstraint.Matches(classOf(source));
				default:
					return equalifier(source, comparisand);
			}
		}

		public static IObject sendMessage(IObject obj, Message message) => classOf(obj).SendMessage(obj, message);

		public static IObject sendMessage(IObject obj, Selector selector, Arguments arguments)
		{
			return classOf(obj).SendMessage(obj, selector, arguments);
		}

		public static IObject sendMessage(IObject obj, Selector selector, params IObject[] arguments)
		{
			return sendMessage(obj, selector, new Arguments(arguments));
		}

		public static IObject sendMessage(IObject obj, Message message, Func<IObject> defaultFunc)
		{
			var cls = classOf(obj);
			if (cls.RespondsTo(message.Selector))
				return cls.SendMessage(obj, message);
			else
				return defaultFunc();
		}

		public static IObject sendMessage(IObject obj, Selector selector, Func<IObject> defaultFunc, Arguments arguments)
		{
			var cls = classOf(obj);
			if (cls.RespondsTo(selector))
				return cls.SendMessage(obj, selector, arguments);
			else
				return defaultFunc();
		}

		public static IObject sendMessage(IObject obj, Selector selector, Func<IObject> defaultFunc, params IObject[] arguments)
		{
			return sendMessage(obj, selector, defaultFunc, new Arguments(arguments));
		}

		public static IObject sendMessage(IObject obj, Selector selector, IObject argument)
		{
			return sendMessage(obj, selector, new Arguments(argument));
		}

		public static string userObjectString(UserObject obj)
		{
			if (classOf(obj).RespondsTo("string".get()))
				return sendMessage(obj, "string".get()).AsString;
			else
				return
					$"object{obj.ObjectID} {obj.ClassName}({obj.Parameters.Select(p => $"{p.Name} = {obj.Fields[p.Name].Image}").Stringify()})";
		}

		public static string userObjectImage(UserObject obj)
		{
			if (classOf(obj).RespondsTo("image".get()))
				return sendMessage(obj, "image".get()).AsString;
			else
				return
					$"object{obj.ObjectID} {obj.ClassName}({obj.Parameters.Select(p => $"{p.Name} = {obj.Fields[p.Name].Image}").Stringify()})";
		}

		public static bool isEqualTo(UserObject obj, IObject other)
		{
			if (classOf(obj).RespondsTo("isEqualTo(_)"))
				return sendMessage(obj, "isEqualTo(_)", other).IsTrue;
			else if (other is UserObject otherUserObject && obj.ClassName == otherUserObject.ClassName)
			{
				var fields = otherUserObject.Fields;
				return obj.Fields.All(f => fields.ContainsKey(f.fieldName) && f.field.Value.IsEqualTo(fields[f.fieldName]));
			}
			else
				return false;
		}

		public static bool userObjectMatch(UserObject obj, IObject comparisand, Hash<string, IObject> bindings)
		{
			bool includeFieldName(string fieldName)
			{
				return !fieldName.StartsWith("__$") && fieldName != "self" && fieldName != "id" && !fieldName.StartsWith("_");
			}

			if (classOf(obj).RespondsTo("match(_,_)"))
			{
				var objectHash = bindings.ToHash(i => String.StringObject(i.Key), i => i.Value);
				var dictionary = new Dictionary(objectHash);
				if (sendMessage(obj, "match(_,_)", comparisand, dictionary).IsTrue)
				{
					var resultHash = dictionary.InternalHash;
					foreach (var (key, value) in resultHash)
						bindings[key.AsString] = value;

					return true;
				}
				else
					return false;
			}
			else
				return match(obj, comparisand, (uo1, uo2) =>
				{
					if (obj.Parameters.Length == 0)
					{
						foreach (var (fieldName, field) in uo1.Fields.Where(f => includeFieldName(f.fieldName)))
						{
							var value1 = field.Value;
							var value2 = uo2.Fields[fieldName];
							if (!value1.Match(value2, bindings))
								return false;
						}

						return true;
					}
					else
						foreach (var parameter in obj.Parameters)
						{
							var name = parameter.Name;
							if (uo1.Fields.ContainsKey(name) && uo2.Fields.ContainsKey(name))
							{
								var value1 = uo1.Fields[name];
								var value2 = uo2.Fields[name];
								if (!value1.Match(value2, bindings))
									return false;
							}
							else
								return false;
						}

					return true;
				}, bindings);
		}

		public static bool processPlaceholdersMatch(IObject obj, IObject comparisand, Hash<string, IObject> bindings)
		{
			if (obj is IProcessPlaceholders ppInternals && comparisand is IProcessPlaceholders ppPassed)
			{
				foreach (var (key, value) in ppInternals.Internals)
					if (ppPassed.Passed.ContainsKey(key))
					{
						var passedValue = ppPassed.Passed[key];
						if (!value.Match(passedValue, bindings))
							return false;
					}
					else
						return false;

				return true;
			}
			else
				return false;
		}

		public static string stringOf(IObject obj)
		{
			var message = "string".get();
			var cls = classOf(obj);
			if (cls.RespondsTo(message))
				return ((String)sendMessage(obj, message)).Value;
			else
				return obj.AsString;
		}

		public static IObject[] setObjects(IObject[] target, IEnumerable<IObject> source, Func<int, IObject> defaultValue)
		{
			var s = source.ToArray();
			var length = Math.Min(target.Length, s.Length);
			var lastValue = Unassigned.Value;
			var lastIndex = -1;

			for (var i = 0; i < length; i++)
			{
				target[i] = s[i];
				lastValue = target[i];
				lastIndex = i;
			}

			if (length < target.Length)
				for (var i = length; i < target.Length; i++)
					target[i] = defaultValue(i);

			else if (length < s.Length)
			{
				var list = new List<IObject> { lastValue };
				for (var i = length; i < s.Length; i++)
					list.Add(s[i]);
				target[lastIndex] = new Tuple(list.ToArray());
			}

			return target;
		}

		public static int wrapIndex(int index, int length) => index > -1 ? index : length + index;

		public static string show(ICollection collection, string begin, Func<IObject, string> func, string end,
			int breakOn = BREAK_EARLY)
		{
			var builder = new StringBuilder(begin);
			var obj = (IObject)collection;

			var rangeSize = collection.Length.Value;
			if (rangeSize == -1)
				builder.Append(func(obj));
			else
			{
				var breakEarly = rangeSize >= breakOn;
				var count = 0;
				var iterator = collection.GetIterator(false);
				var next = iterator.Next();
				if (next.If(out var n))
				{
					builder.Append(func(n));
					next = iterator.Next();
					count++;
				}

				while (next.If(out n))
				{
					builder.Append(", ");
					builder.Append(func(n));
					if (++count == breakOn && breakEarly)
					{
						builder.Append("...");
						break;
					}

					next = iterator.Next();
				}
			}

			builder.Append(end);

			return builder.ToString();
		}

		public static IEnumerable<IObject> list(ICollection collection) => collection.GetIterator(false).List();

		public static IObject someOf(IMaybe<IObject> maybe) => maybe.FlatMap(Some.Object, () => None.NoneValue);

		public static bool compareEnumerables(IEnumerable<IObject> left, IEnumerable<IObject> right)
		{
			var lArray = left.ToArray();
			var rArray = right.ToArray();

			var lLength = lArray.Length;
			var rLength = rArray.Length;
			if (lLength != rLength)
				return false;
			else
			{
				for (var i = 0; i < lLength; i++)
					if (!lArray[i].IsEqualTo(rArray[i]))
						return false;

				return true;
			}
		}

		public static bool compareEnumerables<T>(IEnumerable<T> left, IEnumerable<T> right)
		{
			var lArray = left.ToArray();
			var rArray = right.ToArray();

			var lLength = lArray.Length;
			var rLength = rArray.Length;
			if (lLength != rLength)
				return false;
			else
			{
				for (var i = 0; i < lLength; i++)
					if (!lArray[i].Equals(rArray[i]))
						return false;

				return true;
			}
		}

		public static int compareObjects<T>(T x, IObject y, Func<T, T, int> comparer) where T : IObject
		{
			if (y is T ty)
				return comparer(x, ty);
			else
				throw incompatibleClasses(y, typeof(T).Name);
		}

		public static bool between(IObjectCompare obj, IObject min, IObject max, bool inclusive)
		{
			if (inclusive)
				return obj.Compare(min) >= 0 && obj.Compare(max) <= 0;
			else
				return obj.Compare(min) >= 0 && obj.Compare(max) < 0;
		}

		public static bool after(IObjectCompare obj, IObject min, IObject max, bool inclusive)
		{
			if (inclusive)
				return obj.Compare(min) > 0 && obj.Compare(max) <= 0;
			else
				return obj.Compare(min) > 0 && obj.Compare(max) < 0;
		}

		public static string zfill(string number, int count)
		{
			var sign = "";
			if (number.StartsWith("-"))
			{
				sign = "-";
				number = number.Drop(1);
			}

			return $"{sign}{number.PadLeft(count, '0')}";
		}

		public static Selector parseSelector(string source)
		{
			if (!source.EndsWith(")"))
				source = $"{source}(_)";

			var matcher = new Matcher();
			if (matcher.IsMatch(source, $"^ /(('__$')? {REGEX_FUNCTION_NAME}) '(' /@"))
			{
				var name = matcher.FirstGroup;
				var rest = matcher.SecondGroup.KeepUntil(")");
				SelectorItem[] items;
				if (rest.IsEmpty())
					items = new SelectorItem[0];
				else
				{
					var sourceItems = rest.Split("/s* ',' /s*");
					items = sourceItems.Select(parseSelectorItem).ToArray();
				}

				return new Selector(name, items, source);
			}
			else
				throw $"Can't convert {source} into a Selector".Throws();
		}

		public static SelectorItem parseSelectorItem(string source)
		{
			var label = "";
			var typeConstraint = none<TypeConstraint>();

			var matcher = new Matcher();
			if (matcher.IsMatch(source, $"^ /({REGEX_FIELD}) ':' /@"))
			{
				label = matcher.FirstGroup;
				source = matcher.SecondGroup;
			}

			if (matcher.IsMatch(source, $"^ /({REGEX_FIELD}) /b /@"))
				source = matcher.SecondGroup;

			if (matcher.IsMatch(source, "^ '<' /(-['>']+) '>' /@"))
			{
				var classNames = matcher.FirstGroup.Split("/s+");
				var classes = classNames.Select(cn => Module.Global.Class(cn, true).Required(messageClassNotFound(cn))).ToArray();
				typeConstraint = new TypeConstraint(classes).Some();
				source = matcher.SecondGroup.Trim();
			}

			var selectorItemType = SelectorItemType.Normal;
			switch (source)
			{
				case "...":
					selectorItemType = SelectorItemType.Variadic;
					break;
				case "=":
					selectorItemType = SelectorItemType.Default;
					break;
			}

			return new SelectorItem(label, typeConstraint, selectorItemType);
		}

		public static string selectorImage(string name, SelectorItem[] selectorItems) => $"{name}({selectorItems.Stringify(",")})";

		public static Selector selector(string name, string[] labels, IObject[] objects)
		{
			var enumerable = labels.Zip(objects, (l, o) => $"{l.Extend(after: ":")}_<{o.ClassName}>");
			var selectItems = enumerable.Select(parseSelectorItem).ToArray();
			return new Selector(name, selectItems, selectorImage(name, selectItems));
		}

		public static string formatNumber(int intValue, string format)
		{
			if (format.StartsWith("b"))
			{
				var size = format.Drop(1);
				return ObjectFunctions.format(intValue, size, 2);
			}
			else if (format.StartsWith("o"))
			{
				var size = format.Drop(1);
				return ObjectFunctions.format(intValue, size, 8);
			}
			else
				return intValue.FormatUsing<int>(format, i => i.ToString(format));
		}

		public static string format(int value, int toBase, int size, char padding)
		{
			var result = Convert.ToString(value, toBase);
			return size > 0 ? result.RightJustify(size, padding) : result.LeftJustify(-size, padding);
		}

		public static string format(long value, int toBase, int size, char padding)
		{
			var result = Convert.ToString(value, toBase);
			return size > 0 ? result.RightJustify(size, padding) : result.LeftJustify(-size, padding);
		}

		public static string format(object obj, string size, int toBase)
		{
			if (size.IsEmpty())
			{
				switch (obj)
				{
					case int i:
						return Convert.ToString(i, toBase);
					case long l:
						return Convert.ToString(l, toBase);
					default:
						return obj.ToString();
				}
			}
			else
			{
				var padding = ' ';
				if (size.StartsWith("."))
				{
					size = size.Drop(1);
					padding = '0';
				}

				var length = size.ToInt();

				switch (obj)
				{
					case int i:
						return format(i, toBase, length, padding);
					case long l:
						return format(l, toBase, length, padding);
					default:
						var result = obj.ToString();
						return length > 0 ? result.RightJustify(length, padding) : result.LeftJustify(-length, padding);
				}
			}
		}
	}
}