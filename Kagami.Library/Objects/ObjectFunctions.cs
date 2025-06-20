﻿using System.Text;
using Kagami.Library.Classes;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using Kagami.Library.Parsers;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public static class ObjectFunctions
{
   private const int BREAK_EARLY = 10;

   private static int uniqueID;

   public static void ResetObjectUniqueID() => uniqueID = 0;

   public static int uniqueObjectID() => uniqueID++;

   public static BaseClass classOf(IObject value)
   {
      var className = value.ClassName;
      return Module.Global.Value.Class(className).Required(messageClassNotFound(className));
   }

   public static bool match<T>(T source, IObject comparisand, Func<T, T, bool> equalifier, Hash<string, IObject> bindings)
      where T : IObject
   {
      /*switch (comparisand)
      {
         case Binding binding:
            comparisand = binding.Value;
            bindings[binding.Name] = source;
            break;
         case NameValue nameValue:
            comparisand = nameValue.Value;
            bindings[$"-{nameValue.Name}"] = source;
            break;
      }*/

      var _name = Module.Global.Value.Bindings.Maybe[comparisand.Id];
      if (_name is (true, var name))
      {
         bindings[name] = source;
      }

      switch (comparisand)
      {
         case Any:
            return true;
         case Class cls:
            return classOf(source).MatchCompatible(classOf(cls));
         case Placeholder ph:
            bindings[ph.Name] = source;
            return true;
         case KRange range:
            return range.In(source).IsTrue;
         case Lambda lambda:
            if (lambda.Invoke(source).IsTrue)
            {
               var bindingName = $"-{lambda.Invokable.Parameters[0].Name}";
               bindings[bindingName] = source;
               return true;
            }
            else
            {
               return false;
            }

         case Sequence internalList:
            return internalList.In(source);
         case Regex regex:
            return regex.IsMatch(source.AsString).IsTrue;
         case Pattern pattern:
            return pattern.Match(source, bindings);
         case IProcessPlaceholders:
            return processPlaceholdersMatch(source, comparisand, bindings);
         case TypeConstraint typeConstraint:
            return typeConstraint.Matches(classOf(source));
         default:
            return classOf(source).MatchCompatible(classOf(comparisand)) && equalifier(source, (T)comparisand);
      }
   }

   public static bool match<T>(T source, IObject comparisand, Hash<string, IObject> bindings)
      where T : IObject
   {
      return match(source, comparisand, (x, y) => x.IsEqualTo(y), bindings);
   }

   public static bool matchSingle<T>(T source, IObject comparisand, Func<T, IObject, bool> equalifier,
      Hash<string, IObject> bindings) where T : IObject
   {
      /*switch (comparisand)
      {
         case Binding binding:
            comparisand = binding.Value;
            bindings[binding.Name] = source;
            break;
         case NameValue nameValue:
            comparisand = nameValue.Value;
            bindings[$"-{nameValue.Name}"] = source;
            break;
      }*/

      var _name = Module.Global.Value.Bindings.Maybe[comparisand.Id];
      if (_name is (true, var name))
      {
         bindings[name] = source;
      }

      switch (comparisand)
      {
         case Any:
         case Class:
            return true;
         case Placeholder ph:
            bindings[ph.Name] = source;
            return true;
         case KRange range:
            return range.In(source).IsTrue;
         case Lambda lambda:
            if (lambda.Invoke(source).IsTrue)
            {
               var bindingName = $"-{lambda.Invokable.Parameters[0].Name}";
               bindings[bindingName] = source;
               return true;
            }
            else
            {
               return false;
            }

         case Pattern pattern:
            return pattern.Match(source, bindings);
         case IProcessPlaceholders:
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
      {
         return cls.SendMessage(obj, message);
      }
      else
      {
         return defaultFunc();
      }
   }

   public static IObject sendMessage(IObject obj, Selector selector, Func<IObject> defaultFunc, Arguments arguments)
   {
      var cls = classOf(obj);
      if (cls.RespondsTo(selector))
      {
         return cls.SendMessage(obj, selector, arguments);
      }
      else
      {
         return defaultFunc();
      }
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
      {
         return sendMessage(obj, "string".get()).AsString;
      }
      else
      {
         var parametersAndFields = obj.Parameters.Select(p => $"{p.Name} = {obj.Fields[p.Name].Image}").ToString(", ");
         return $"object{obj.ObjectID} {obj.ClassName}({parametersAndFields})";
      }
   }

   public static string userObjectImage(UserObject obj)
   {
      if (classOf(obj).RespondsTo("image".get()))
      {
         return sendMessage(obj, "image".get()).AsString;
      }
      else
      {
         var parametersAndFields = obj.Parameters.Select(p => $"{p.Name} = {obj.Fields[p.Name].Image}").ToString(", ");
         return $"object{obj.ObjectID} {obj.ClassName}({parametersAndFields})";
      }
   }

   public static bool isEqualTo(UserObject obj, IObject other)
   {
      if (classOf(obj).RespondsTo("isEqualTo(_)"))
      {
         return sendMessage(obj, "isEqualTo(_)", other).IsTrue;
      }
      /*else if (other is UserObject otherUserObject && obj.ClassName == otherUserObject.ClassName)
      {
         var fields = otherUserObject.Fields;
         return obj.Fields.All(f => fields.ContainsKey(f.fieldName) && f.field.Value.IsEqualTo(fields[f.fieldName]));
      }*/
      else
      {
         return false;
      }
   }

   public static bool userObjectMatch(UserObject obj, IObject comparisand, Hash<string, IObject> bindings)
   {
      static bool includeFieldName(string fieldName)
      {
         return !fieldName.StartsWith("__$") && fieldName != "self" && fieldName != "id" && !fieldName.StartsWith("_");
      }

      if (classOf(obj).RespondsTo("match(_,_)"))
      {
         var objectHash = bindings.ToHash(i => KString.StringObject(i.Key), i => i.Value);
         var dictionary = new Dictionary(objectHash);
         if (sendMessage(obj, "match(_,_)", comparisand, dictionary).IsTrue)
         {
            var resultHash = dictionary.InternalHash;
            foreach (var (key, value) in resultHash)
            {
               bindings[key.AsString] = value;
            }

            return true;
         }
         else
         {
            return false;
         }
      }
      else
      {
         return match(obj, comparisand, (uo1, uo2) =>
         {
            if (obj.Parameters.Length == 0)
            {
               foreach (var (fieldName, field) in uo1.Fields.Where(f => includeFieldName(f.fieldName)))
               {
                  var value1 = field.Value;
                  var value2 = uo2.Fields[fieldName];
                  if (!value1.Match(value2, bindings))
                  {
                     return false;
                  }
               }
            }
            else
            {
               foreach (var parameter in obj.Parameters)
               {
                  var name = parameter.Name;
                  if (uo1.Fields.ContainsKey(name) && uo2.Fields.ContainsKey(name))
                  {
                     var value1 = uo1.Fields[name];
                     var value2 = uo2.Fields[name];
                     if (!value1.Match(value2, bindings))
                     {
                        return false;
                     }
                  }
                  else
                  {
                     return false;
                  }
               }
            }

            return true;
         }, bindings);
      }
   }

   public static bool processPlaceholdersMatch(IObject obj, IObject comparisand, Hash<string, IObject> bindings)
   {
      if (obj is IProcessPlaceholders ppInternals && comparisand is IProcessPlaceholders ppPassed)
      {
         foreach (var (key, value) in ppInternals.Internals)
         {
            if (ppPassed.Passed.Maybe[key] is (true, var passedValue))
            {
               if (!value.Match(passedValue, bindings))
               {
                  return false;
               }
            }
            else
            {
               return false;
            }
         }

         return true;
      }
      else
      {
         return false;
      }
   }

   public static string stringOf(IObject obj)
   {
      var message = "string".get();
      var cls = classOf(obj);
      return cls.RespondsTo(message) ? ((KString)sendMessage(obj, message)).Value : obj.AsString;
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
      {
         for (var i = length; i < target.Length; i++)
         {
            target[i] = defaultValue(i);
         }
      }

      else if (length < s.Length)
      {
         var list = new List<IObject> { lastValue };
         for (var i = length; i < s.Length; i++)
         {
            list.Add(s[i]);
         }

         target[lastIndex] = new KTuple(list.ToArray());
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
      {
         builder.Append(func(obj));
      }
      else
      {
         var breakEarly = rangeSize >= breakOn;
         var count = 0;
         var iterator = collection.GetIterator(false);
         var _next = iterator.Next();
         if (_next is (true, var next))
         {
            builder.Append(func(next));
            _next = iterator.Next();
            count++;
         }

         while (_next is (true, var next2))
         {
            builder.Append(", ");
            builder.Append(func(next2));
            if (++count == breakOn && breakEarly)
            {
               builder.Append("...");
               break;
            }

            _next = iterator.Next();
         }
      }

      builder.Append(end);

      return builder.ToString();
   }

   public static IEnumerable<IObject> list(ICollection collection) => collection.GetIterator(false).List();

   public static IObject someOf(Maybe<IObject> maybe) => maybe.Map(Some.Object) | (() => None.NoneValue);

   public static bool compareEnumerables(IEnumerable<IObject> left, IEnumerable<IObject> right)
   {
      var lArray = left.ToArray();
      var rArray = right.ToArray();

      var lLength = lArray.Length;
      var rLength = rArray.Length;
      if (lLength != rLength)
      {
         return false;
      }
      else
      {
         for (var i = 0; i < lLength; i++)
         {
            if (!lArray[i].IsEqualTo(rArray[i]))
            {
               return false;
            }
         }

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
      {
         return false;
      }
      else
      {
         for (var i = 0; i < lLength; i++)
         {
            if (!lArray[i]!.Equals(rArray[i]))
            {
               return false;
            }
         }

         return true;
      }
   }

   public static int compareObjects<T>(T x, IObject y, Func<T, T, int> comparer) where T : IObject
   {
      if (y is T ty)
      {
         return comparer(x, ty);
      }
      else
      {
         throw incompatibleClasses(y, typeof(T).Name);
      }
   }

   public static bool between(IObjectCompare obj, IObject min, IObject max, bool inclusive)
   {
      if (inclusive)
      {
         return obj.Compare(min) >= 0 && obj.Compare(max) <= 0;
      }
      else
      {
         return obj.Compare(min) >= 0 && obj.Compare(max) < 0;
      }
   }

   public static bool after(IObjectCompare obj, IObject min, IObject max, bool inclusive)
   {
      if (inclusive)
      {
         return obj.Compare(min) > 0 && obj.Compare(max) <= 0;
      }
      else
      {
         return obj.Compare(min) > 0 && obj.Compare(max) < 0;
      }
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
      if (source.MatchOf(@$"^((?:__\$)?{REGEX_FUNCTION_NAME})(.*)$") is (true, var matches))
      {
         var match = matches[0];
         var name = match.Groups[1].Value;
         var rest = match.Groups[2].Value;

         if (!name.StartsWith("__$") && rest.IsEmpty())
         {
            //rest = name.EndsWith('=') ? "(_)" : "()";
            name = $"__${name}";
         }

         rest = rest.Substitute("^ '(' /(-[')']+) ')' $", "$1");
         SelectorItem[] items;
         if (rest.IsEmpty() || rest == "()")
         {
            items = [];
         }
         else
         {
            var sourceItems = rest.Unjoin("/s* ',' /s*");
            items = sourceItems.Select(parseSelectorItem).ToArray();
         }

         return new Selector(name, items, selectorImage(name, items));
      }
      else
      {
         throw selectorIncorrectFormat(source);
      }
   }

   public static SelectorItem parseSelectorItem(string source)
   {
      var label = "";
      Maybe<TypeConstraint> _typeConstraint = nil;

      if (source.MatchOf($"^({REGEX_FIELD}):(.*)$") is (true, var matches))
      {
         var match = matches[0];
         label = match.Groups[1].Value;
         source = match.Groups[2].Value;
      }

      if (source.MatchOf($@"^({REGEX_FIELD})\b(.*)$") is (true, var matches2))
      {
         var match = matches2[0];
         source = match.Groups[2].Value;
      }

      if (source.MatchOf("^<([^>]+)>(.*)$") is (true, var matches3))
      {
         var match = matches3[0];
         var classNames = match.Groups[1].Value.Unjoin("/s+");
         var classes = classNames.Select(cn => Module.Global.Value.Class(cn, true).Required(messageClassNotFound(cn))).ToArray();
         _typeConstraint = new TypeConstraint(classes);
         source = match.Groups[2].Value.Trim();
      }

      var selectorItemType = source switch
      {
         "..." => SelectorItemType.Variadic,
         "=" => SelectorItemType.Default,
         _ => SelectorItemType.Normal
      };

      return new SelectorItem(label, _typeConstraint, selectorItemType);
   }

   public static string selectorImage(string name, SelectorItem[] selectorItems) => $"{name}({selectorItems.ToString(",")})";

   public static Selector selector(string name, string[] labels, IObject[] objects)
   {
      var enumerable = labels.Zip(objects, (l, o) => (l.IsNotEmpty() ? $"{l}:" : "") + $"_<{o.ClassName}>");
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
      {
         return intValue.FormatUsing<int>(format, i => i.ToString(format).Replace("@", "e"));
      }
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
         return obj switch
         {
            int i => Convert.ToString(i, toBase),
            long l => Convert.ToString(l, toBase),
            _ => obj.ToString() ?? ""
         };
      }
      else
      {
         var padding = ' ';
         if (size.StartsWith("."))
         {
            size = size.Drop(1);
            padding = '0';
         }

         var length = size.Value().Int32();

         switch (obj)
         {
            case int i:
               return format(i, toBase, length, padding);
            case long l:
               return format(l, toBase, length, padding);
            default:
               var result = obj.ToString() ?? "";
               return length > 0 ? result.RightJustify(length, padding) : result.LeftJustify(-length, padding);
         }
      }
   }

   public static IObject assignToMutable(IObject collection, IObject possibleSkipTake, IObject source)
   {
      if (collection is IMutableCollection mutableCollection)
      {
         if (possibleSkipTake is SkipTake skipTake)
         {
            switch (source)
            {
               case Some some:
               {
                  List<IObject> list = [some.Value];
                  return mutableCollection.Assign(skipTake, list);
               }
               case ICollection sourceCollection:
               {
                  var enumerable = sourceCollection.GetIterator(false).List();
                  return mutableCollection.Assign(skipTake, enumerable);
               }
               default:
                  throw fail("Source must be a collection");
            }
         }
         else
         {
            throw fail("Index must be a skip and take");
         }
      }
      else
      {
         throw fail("Target must be a mutable collection");
      }
   }
}