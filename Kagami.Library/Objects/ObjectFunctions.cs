using Core.Collections;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using Kagami.Library.Classes;
using Kagami.Library.Parsers;
using Kagami.Library.Runtime;
using System.Text;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.CommonFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

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
      return classOf(className);
   }

   public static BaseClass classOf(string className) => Module.Global.Value.Class(className).Required(messageClassNotFound(className));

   public static bool match<T>(T source, IObject comparisand, Func<T, T, bool> equalifier, Hash<string, IObject> bindings)
      where T : IObject
   {
      if (comparisand is NameValue nameValue)
      {
         bindings["-" + nameValue.Key.AsString] = source;
         comparisand = nameValue.Value;
      }

      var _name = Module.Global.Value.Bindings.Maybe[comparisand.Id];
      if (_name is (true, var name))
      {
         bindings[name] = source;
      }

      if (comparisand.ClassName == "Number" && source is INumeric)
      {
         return true;
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

         case Sequence sequence:
            return matchInSequence(source, sequence, bindings);
         case Regex regex:
            return matchRegex(source, regex, bindings);
         case Pattern pattern:
            return pattern.Match(source, bindings);
         case IProcessPlaceholders:
            return processPlaceholdersMatch(source, comparisand, bindings);
         case TypeConstraint typeConstraint:
            return typeConstraint.Matches(classOf(source));
         case Cons cons when source is KArray array:
            return matchArrayToCons(array, cons, bindings);
         case KTuple tuple when source is KArray array:
            return matchArrayToTuple(array, tuple, bindings);
         case KTuple tuple when source is KString kString:
            return matchStringToTuple(kString, tuple, bindings);
         /*case KTuple tuple when source is not KTuple:
            return matchNonTuple(source, tuple, bindings);*/
         case KTuple tuple1 when source is KTuple tuple2:
            return matchTupleToTuple(tuple2, tuple1, bindings);
         case SpecialComparisand specialComparisand:
            return specialComparisand.Match(source, bindings);
         case UserObject userObjectSource when source is UserObject userObject:
         {
            return userObjectMatch(userObjectSource, userObject, bindings);
         }
         default:
            return classOf(source).MatchCompatible(classOf(comparisand)) && equalifier(source, (T)comparisand);
      }
   }

   private static bool matchRegex(IObject source, Regex regex, Hash<string, IObject> bindings)
   {
      var _match = regex.MatchOne(source.AsString);
      if (_match is (true, var match))
      {
         var _name = Module.Global.Value.Bindings.Maybe[regex.Id];
         if (_name is (true, var name))
         {
            bindings[name] = (KString)match.Text;
         }

         return true;
      }
      else
      {
         return false;
      }
   }

   private static bool matchNonTuple(IObject source, KTuple tuple, Hash<string, IObject> bindings)
   {
      foreach (var (index, item) in tuple.List.Indexed())
      {
         var isMatch = match(source, item, bindings);
         if (isMatch)
         {
            var _name = tuple.Rename(index);
            if (_name is (true, var name))
            {
               bindings[$"-{name}"] = source;
            }
         }
         else
         {
            return false;
         }
      }

      return true;
   }

   private static bool matchTupleToTuple(KTuple source, KTuple comparisand, Hash<string, IObject> bindings)
   {
      var length = source.Length.Value;
      if (length != comparisand.Length.Value)
      {
         return false;
      }

      for (var i = 0; i < length; i++)
      {
         var item1 = source[i];
         var item2 = comparisand[i];

         var matched = item1.Match(item2, bindings);
         if (!matched)
         {
            return false;
         }
      }

      return true;
   }

   private static bool matchInSequence(IObject source, Sequence sequence, Hash<string, IObject> bindings)
   {
      return sequence.List.Any(item => match(source, item, bindings));
   }

   private static bool matchArrayToCons(KArray source, Cons cons, Hash<string, IObject> bindings)
   {
      var head = source.Head;
      if (head is KNil)
      {
         return false;
      }

      var tail = source.Tail;

      var match0 = cons.Head;
      var match1 = cons.Tail;

      switch (match0)
      {
         case Any:
            return true;
         case Placeholder placeholder0 when head is Some some:
         {
            bindings[placeholder0.Name] = some.Value;
            break;
         }
         default:
         {
            if (head is Some some && !some.Value.Match(match0, bindings))
            {
               return false;
            }

            break;
         }
      }

      switch (match1)
      {
         case Any:
            return true;
         case Placeholder placeholder1:
            bindings[placeholder1.Name] = tail;
            return true;
         case KArray array:
            return tail.Match(array, bindings);
      }

      return false;
   }

   private static bool matchArrayToTuple(KArray source, KTuple comparisand, Hash<string, IObject> bindings)
   {
      if (comparisand.Length.Value == 2)
      {
         var head = source.Head;
         if (head is KNil)
         {
            return false;
         }

         var tail = source.Tail;

         var match0 = comparisand[0];
         var match1 = comparisand[1];

         switch (match0)
         {
            case Placeholder placeholder0 when head is Some some:
            {
               bindings[placeholder0.Name] = some.Value;
               break;
            }
            default:
            {
               if (head is Some some && !some.Value.Match(match0, bindings))
               {
                  return false;
               }

               break;
            }
         }

         switch (match1)
         {
            case Placeholder placeholder1:
               bindings[placeholder1.Name] = tail;
               break;
            case KArray array1 when !array1.IsEqualTo(tail):
               return false;
            case KTuple tuple:
               return match(tail, tuple, bindings);
         }

         return true;
      }

      return false;
   }

   private static bool matchStringToTuple(KString source, KTuple comparisand, Hash<string, IObject> bindings)
   {
      if (comparisand.Length.Value == 2)
      {
         var head = source.Head;
         if (head.IsEmpty.Value)
         {
            return false;
         }

         var tail = source.Tail;

         var match0 = comparisand[0];
         var match1 = comparisand[1];

         switch (match0)
         {
            case Placeholder placeholder0 when head.IsNotEmpty.Value:
            {
               bindings[placeholder0.Name] = head;
               break;
            }
            default:
            {
               if (head.IsNotEmpty.Value && !head.Match(match0, bindings))
               {
                  return false;
               }

               break;
            }
         }

         switch (match1)
         {
            case Placeholder placeholder1:
               bindings[placeholder1.Name] = tail;
               break;
            case KString kString when !kString.IsEqualTo(tail):
               return false;
         }

         return true;
      }

      return false;
   }

   public static bool match<T>(T source, IObject comparisand, Hash<string, IObject> bindings)
      where T : IObject
   {
      return match(source, comparisand, (x, y) => x.IsEqualTo(y), bindings);
   }

   public static IObject sendMessage(IObject obj, Message message) => classOf(obj).SendMessage(obj, message);

   public static IObject sendMessage(IObject obj, string selector, Arguments arguments)
   {
      return classOf(obj).SendMessage(obj, selector, arguments);
   }

   public static IObject sendMessage(IObject obj, string selector, params IObject[] arguments)
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

   public static IObject sendMessage(IObject obj, string selector, IObject argument)
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
         var parametersAndFields = obj.Parameters.Select(p => $"{p.Name} = {obj.Fields[p.Name].AsString}").ToString(", ");
         return $"{obj.ClassName}({parametersAndFields})";
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
         return $"{obj.ClassName}({parametersAndFields})<{shortenedId(obj.Id)}>";
      }
   }

   private static bool userObjectMatches(UserObject source, UserObject comparisand, Hash<string, IObject> bindings)
   {
      if (source.ClassName == comparisand.ClassName)
      {
         if (source.Parameters.Length != comparisand.Parameters.Length)
         {
            return false;
         }

         foreach (var parameter in source.Parameters)
         {
            if (!comparisand.Fields.ContainsKey(parameter.Name) ||
                !source.Fields[parameter.Name].Match(comparisand.Fields[parameter.Name], bindings))
            {
               return false;
            }
         }

         return true;
      }

      return false;
   }

   public static bool isEqualTo(UserObject obj, IObject other)
   {
      if (classOf(obj).RespondsTo("isEqualTo(_)"))
      {
         return sendMessage(obj, "isEqualTo(_)", other).IsTrue;
      }
      else if (other is UserObject otherUserObject)
      {
         if (otherUserObject.ClassName == obj.ClassName)
         {
            if (obj.Parameters.Length != otherUserObject.Parameters.Length)
            {
               return false;
            }

            foreach (var parameter in obj.Parameters)
            {
               if (!otherUserObject.Fields.ContainsKey(parameter.Name) ||
                   !obj.Fields[parameter.Name].IsEqualTo(otherUserObject.Fields[parameter.Name]))
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
      else
      {
         return false;
      }
   }

   public static bool userObjectMatch(UserObject obj, IObject comparisand, Hash<string, IObject> bindings)
   {
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
      else if (comparisand is UserObjectPlaceholder userObjectPlaceholder)
      {
         return userObjectPlaceholder.Match(obj, bindings);
      }
      else if (comparisand is UserObject uoComparisand)
      {
         if (obj.ClassName != uoComparisand.ClassName)
         {
            return false;
         }

         if (obj.Parameters.Length > 0)
         {
            foreach (var parameter in obj.Parameters)
            {
               var name = parameter.Name;
               if (obj.Fields.ContainsKey(name) && uoComparisand.Fields.ContainsKey(name))
               {
                  var value1 = obj.Fields[name];
                  var value2 = uoComparisand.Fields[name];
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
      }

      return true;
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

   public static IObject someOf(Maybe<IObject> maybe) => maybe.Map(Some.Object) | (() => KNil.NilValue);

   public static IObject successOf(Result<IObject> result)
   {
      if (result is (true, var success))
      {
         return Success.Object(success);
      }
      else
      {
         return new Failure(result.Exception.Message);
      }
   }

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

   public static int compareObjects(IObject x, IObject y)
   {
      if (x is IObjectCompare objectCompare)
      {
         return objectCompare.Compare(y);
      }
      else
      {
         throw incompatibleClasses(x, "ObjectCompare");
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
      if (source.MatchOf(@$"^((?:__\$)?{REGEX_FUNCTION_NAME2})(.*)$") is (true, var matches))
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
         return intValue.FormatUsing(format, i => i.ToString(format).Replace("@", "e"));
      }
   }

   public static string formatNumber(int intValue, string[] formats)
   {
      switch (formats.Length)
      {
         case 0:
            return intValue.ToString();
         case 1:
            return formatNumber(intValue, formats[0]);
         default:
         {
            var result = formatNumber(intValue, formats[0]);
            foreach (var format in formats.Skip(1))
            {
               result = result.FormatUsing(format, r => r);
            }

            return result;
         }
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

   public static KString format(IFormattable formattable, string[] formats)
   {
      switch (formats.Length)
      {
         case 0:
            return formattable.ToString() ?? "";
         case 1:
            return formattable.Format(formats[0]);
         default:
         {
            var result = formattable.Format(formats[0]);
            foreach (var format in formats.Skip(1))
            {
               result = result.Value.FormatUsing(format, s => s);
            }

            return result;
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

   public static IObject getConstructor(Selector selector)
   {
      var machine = Machine.Current.Value;
      var _field = machine.Find(selector);
      if (_field is (true, { Value: Constructor constructor }))
      {
         return constructor;
      }
      else if (_field.Exception is (true, var exception))
      {
         throw exception;
      }
      else
      {
         throw fail($"Constructor {selector} not found");
      }
   }

   public static IObject createObject(Selector selector, Message message)
   {
      var machine = Machine.Current.Value;
      var _field = machine.Find(selector);
      if (_field is (true, { Value: Constructor constructor }))
      {
         return machine.Invoke(constructor.Invokable, message.Arguments, nil).Force();
      }
      else if (_field.Exception is (true, var exception))
      {
         throw exception;
      }
      else
      {
         throw fail($"Constructor {selector} not found");
      }
   }

   public static IObject pipeline(IObject argument, IObject action)
   {
      switch (action)
      {
         case Lambda lambda:
            return lambda.Invoke(argument);
         case IMayInvoke mayInvoke:
            return mayInvoke.Invoke(argument);
         case Message message:
            return classOf(argument).SendMessage(argument, message);
         case Selector selector:
         {
            var _field = Machine.Current.Value.Find(selector);
            if (_field is (true, { Value: Lambda lambda }))
            {
               return lambda.Invoke(argument);
            }
            else if (_field.Exception is (true, var exception))
            {
               throw exception;
            }
            else
            {
               throw fieldNotFound(selector);
            }
         }
         default:
            throw incompatibleClasses(action, "Lambda or Message");
      }
   }

   public static bool matchingTypeConstraints(Maybe<TypeConstraint> _left, Maybe<TypeConstraint> _right)
   {
      if (_left is (true, var left))
      {
         if (_right is (true, var right))
         {
            return left.IsEqualTo(right);
         }
      }
      else
      {
         if (!_right)
         {
            return true;
         }
      }

      return false;
   }

   public static Optional<int> compareByMessage(IObject left, IObject right)
   {
      var result = sendMessage(left, new Message("<>(_)", right));
      if (result is INumeric numeric)
      {
         return numeric.AsInt32();
      }
      else
      {
         return incompatibleClasses(result, "Int");
      }
   }

   public static Optional<IObject> lessThan(IObject left, IObject right)
   {
      return compareByMessage(left, right).Map(i => KBoolean.BooleanObject(i < 0));
   }

   public static Optional<IObject> lessThanEqual(IObject left, IObject right)
   {
      return compareByMessage(left, right).Map(i => KBoolean.BooleanObject(i <= 0));
   }

   public static Optional<IObject> greaterThan(IObject left, IObject right)
   {
      return compareByMessage(left, right).Map(i => KBoolean.BooleanObject(i > 0));
   }

   public static Optional<IObject> greaterThanEqual(IObject left, IObject right)
   {
      return compareByMessage(left, right).Map(i => KBoolean.BooleanObject(i >= 0));
   }

   public static Optional<IObject> equal(IObject left, IObject right)
   {
      return compareByMessage(left, right).Map(i => KBoolean.BooleanObject(i == 0));
   }

   public static Optional<IObject> notEqual(IObject left, IObject right)
   {
      return compareByMessage(left, right).Map(i => KBoolean.BooleanObject(i != 0));
   }

   public static Maybe<IObject> maybe(IObject obj) => obj is Some some ? some.Value.Some() : nil;
}