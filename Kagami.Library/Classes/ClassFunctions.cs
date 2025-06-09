using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Collections;

namespace Kagami.Library.Classes;

public static class ClassFunctions
{
   public static IObject function<T>(IObject x, Func<T, IObject> func)
      where T : IObject => func((T)x);

   public static IObject function<T1, T2>(IObject x, Message message, Func<T1, T2, IObject> func)
      where T1 : IObject
      where T2 : IObject => func((T1)x, (T2)message.Arguments[0]);

   public static IObject function<T1, T2, T3>(IObject x, Message message, Func<T1, T2, T3, IObject> func)
      where T1 : IObject
      where T2 : IObject
      where T3 : IObject => func((T1)x, (T2)message.Arguments[0], (T3)message.Arguments[1]);

   public static IObject function<T1, T2, T3, T4>(IObject x, Message message, Func<T1, T2, T3, T4, IObject> func)
      where T1 : IObject
      where T2 : IObject
      where T3 : IObject
      where T4 : IObject => func((T1)x, (T2)message.Arguments[0], (T3)message.Arguments[1], (T4)message.Arguments[2]);

   public static IObject function<T1, T2, T3, T4, T5>(IObject x, Message message, Func<T1, T2, T3, T4, T5, IObject> func)
      where T1 : IObject
      where T2 : IObject
      where T3 : IObject
      where T4 : IObject
      where T5 : IObject => func((T1)x, (T2)message.Arguments[0], (T3)message.Arguments[1], (T4)message.Arguments[2],
      (T5)message.Arguments[3]);

   public static IObject classFunc<T>(BaseClass cls, Func<T, IObject> func)
      where T : BaseClass => func((T)cls);

   public static IObject classFunc<T1, T2>(BaseClass cls, Message message, Func<T1, T2, IObject> func)
      where T1 : BaseClass
      where T2 : IObject => func((T1)cls, (T2)message.Arguments[0]);

   public static IObject classFunc<T1, T2, T3>(BaseClass cls, Message message, Func<T1, T2, T3, IObject> func)
      where T1 : BaseClass
      where T2 : IObject
      where T3 : IObject => func((T1)cls, (T2)message.Arguments[0], (T3)message.Arguments[1]);

   public static IObject collectionFunc(IObject x, Func<ICollection, IObject> func) => func((ICollection)x);

   public static IObject collectionFunc<T>(IObject x, Message message, Func<ICollection, T, IObject> func)
      where T : IObject => func((ICollection)x, (T)message.Arguments[0]);

   public static IObject collectionFunc<T1, T2>(IObject x, Message message, Func<ICollection, T1, T2, IObject> func)
      where T1 : IObject
      where T2 : IObject => func((ICollection)x, (T1)message.Arguments[0], (T2)message.Arguments[1]);

   public static IObject iteratorFunc(IObject x, Func<IIterator, IObject> func) => func((IIterator)x);

   public static IObject iteratorFunc<T>(IObject x, Message message, Func<IIterator, T, IObject> func)
      where T : IObject => func((IIterator)x, (T)message.Arguments[0]);

   public static IObject iteratorFunc<T1, T2>(IObject x, Message message, Func<IIterator, T1, T2, IObject> func)
      where T1 : IObject
      where T2 : IObject => func((IIterator)x, (T1)message.Arguments[0], (T2)message.Arguments[1]);

   public static IObject match(IObject obj, Message message)
   {
      var bindings = new Hash<string, IObject>();
      var result = obj.Match(message.Arguments[0], bindings);
      if (result)
      {
         Machine.Fields.SetBindings(bindings);
         return KBoolean.True;
      }
      else
      {
         return KBoolean.False;
      }
   }

   public static IObject msgNumberFunction(IObject x, Func<IMessageNumber, IObject> func) => func((IMessageNumber)x);

   public static IObject msgNumberFunction(IObject x, Message message, Func<IMessageNumber, INumeric, IObject> func)
   {
      var left = (INumeric)x;
      var right = (INumeric)message.Arguments[0];
      (left, right) = left.Compatible(right);
      return func((IMessageNumber)left, right);
   }
}