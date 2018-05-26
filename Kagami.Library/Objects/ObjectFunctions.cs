using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kagami.Library.Classes;
using Kagami.Library.Runtime;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;

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
         if (comparisand is Binding binding)
         {
            comparisand = binding.Value;
            bindings[binding.Name] = source;
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
                  bindings[lambda.Invokable.Parameters[0].Name] = source;
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

      public static bool matchSingle<T>(T source, IObject comparisand, Func<T, IObject, bool> equalifier,
         Hash<string, IObject> bindings)
         where T : IObject
      {
         if (comparisand is Binding binding)
         {
            comparisand = binding.Value;
            bindings[binding.Name] = source;
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
                  bindings[lambda.Invokable.Parameters[0].Name] = source;
                  return true;
               }
               else
                  return false;
            case Pattern pattern:
               return pattern.Match(source, bindings);
            default:
               return equalifier(source, comparisand);
         }
      }

      public static IObject sendMessage(IObject obj, Message message) => classOf(obj).SendMessage(obj, message);

      public static IObject sendMessage(IObject obj, string messageName, Arguments arguments)
      {
         return classOf(obj).SendMessage(obj, messageName, arguments);
      }

      public static IObject sendMessage(IObject obj, string messageName, params IObject[] arguments)
      {
         return sendMessage(obj, messageName, new Arguments(arguments));
      }

      public static IObject sendMessage(IObject obj, Message message, Func<IObject> defaultFunc)
      {
         var cls = classOf(obj);
         if (cls.RespondsTo(message.Name))
            return cls.SendMessage(obj, message);
         else
            return defaultFunc();
      }

      public static IObject sendMessage(IObject obj, string messageName, Func<IObject> defaultFunc, Arguments arguments)
      {
         var cls = classOf(obj);
         if (cls.RespondsTo(messageName))
            return cls.SendMessage(obj, messageName, arguments);
         else
            return defaultFunc();
      }

      public static IObject sendMessage(IObject obj, string messageName, Func<IObject> defaultFunc, params IObject[] arguments)
      {
         return sendMessage(obj, messageName, defaultFunc, new Arguments(arguments));
      }

      public static IObject sendMessage(IObject obj, string messageName, IObject argument)
      {
         return sendMessage(obj, messageName, new Arguments(argument));
      }

      public static string userObjectString(UserObject obj)
      {
         return
            $"object{obj.ObjectID} {obj.ClassName}({obj.Parameters.Select(p => $"{p.Name} = {obj.Fields[p.Name].Image}").Listify()})";
      }

      public static bool isEqualTo(UserObject obj, IObject other)
      {
         if (other is UserObject otherObject)
            return obj.ClassName == otherObject.ClassName && obj.Fields.Where(f => f.fieldName != "self")
               .Equals(otherObject.Fields.Where(f => f.fieldName != "self"));
         else
            return false;
      }

      public static bool userObjectMatch(UserObject obj, IObject comparisand, Hash<string, IObject> bindings)
      {
         if (classOf(obj).RespondsTo("match"))
         {
            var objectHash = bindings.ToHash(i => String.Object(i.Key), i => i.Value);
            var dictionary = new Dictionary(objectHash);
            if (sendMessage(obj, "match", dictionary).IsTrue)
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
            foreach (var parameter in obj.Parameters)
            {
               var name = parameter.Name;
               if (obj.Fields.ContainsKey(name) && uo2.Fields[name] is Placeholder ph)
                  bindings[ph.Name] = obj.Fields[name];
            }

            return true;
         }, bindings);
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

      public static IObject someOf(IMaybe<IObject> maybe) => maybe.FlatMap(Some.Object, () => Nil.NilValue);

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

      public static int compareObjects<T>(T x, IObject y, Func<T, T, int> comparer) where T : IObject
      {
         if (y is T ty)
            return comparer(x, ty);
         else
            throw incompatibleClasses(y, typeof(T).Name);
      }
   }
}