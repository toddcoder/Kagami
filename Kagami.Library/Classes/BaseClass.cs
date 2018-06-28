using System;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Collections;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Classes.ClassFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Operations.NumericFunctions;
using static Standard.Types.Arrays.ArrayFunctions;
using Boolean = Kagami.Library.Objects.Boolean;
using String = Kagami.Library.Objects.String;
using Byte = Kagami.Library.Objects.Byte;
using IFormattable = Kagami.Library.Objects.IFormattable;
using Tuple = Kagami.Library.Objects.Tuple;
using Void = Kagami.Library.Objects.Void;

namespace Kagami.Library.Classes
{
   public abstract class BaseClass
   {
      protected Hash<string, Func<IObject, Message, IObject>> messages;
      protected Hash<string, Func<BaseClass, Message, IObject>> classMessages;
      protected bool registered;
      protected bool classRegistered;
      protected Set<string> alternateMessages;
      protected Func<IObject, Message, IObject> dynamicInvoke;
      protected Func<Message, IObject> classDynamicInvoke;
      protected Fields classFields;

      public BaseClass()
      {
         messages = new Hash<string, Func<IObject, Message, IObject>>();
         classMessages = new Hash<string, Func<BaseClass, Message, IObject>>();
         registered = false;
         classRegistered = false;
         alternateMessages = new Set<string>();
         dynamicInvoke = (obj, msg) => throw messageNotFound(classOf(obj), msg.Name);
         classDynamicInvoke = msg => throw messageNotFound(this, msg.Name);
         classFields = new Fields();
      }

      public abstract string Name { get; }

      public Fields ClassFields => classFields;

      public virtual IObject DynamicInvoke(IObject obj, Message message) => dynamicInvoke(obj, message);

      public virtual IObject ClassDynamicInvoke(Message message) => classDynamicInvoke(message);

      public virtual bool DynamicRespondsTo(string message) => alternateMessages.Contains(message);

      public virtual bool ClassDynamicRespondsTo(string message) => false;

      protected void registerMessage(string name, Func<IObject, Message, IObject> function)
      {
         if (!messages.ContainsKey(name))
            messages[name] = function;
      }

      protected void registerClassMessage(string name, Func<BaseClass, Message, IObject> function)
      {
         if (!classMessages.ContainsKey(name))
            classMessages[name] = function;
      }

      public virtual void RegisterMessages()
      {
         registerMessage("string".get(), (obj, msg) => String.StringObject(obj.AsString));
         registerMessage("image".get(), (obj, msg) => String.StringObject(obj.Image));
         registerMessage("hash".get(), (obj, msg) => Int.IntObject(obj.Hash));
         registerMessage("equals", (obj, msg) => Boolean.BooleanObject(obj.IsEqualTo(msg.Arguments[0])));
         registerMessage("className".get(), (obj, msg) => String.StringObject(obj.ClassName));
         registerMessage("class".get(), (obj, msg) => new Class(obj.ClassName));
         registerMessage("match", match);
      }

      public virtual void RegisterClassMessages()
      {
         registerClassMessage("name".get(), (cls, msg) => String.StringObject(Name));
      }

      public virtual void RegisterMessage(string name, Func<IObject, Message, IObject> func) => messages[name] = func;

      public void RegisterClassMessage(string name, Func<BaseClass, Message, IObject> func) => classMessages[name] = func;

      protected virtual void registerIfUnregistered()
      {
         if (!registered)
         {
            RegisterMessages();
            registered = true;
         }
      }

      protected virtual void registerClassIfUnregistered()
      {
         if (!classRegistered)
         {
            RegisterClassMessages();
            classRegistered = true;
         }
      }

      public virtual bool RespondsTo(string message)
      {
         registerIfUnregistered();

         return messages.ContainsKey(message) || DynamicRespondsTo(message);
      }

      public virtual bool ClassRespondsTo(string message)
      {
         registerClassIfUnregistered();

         return classMessages.ContainsKey(message) || ClassDynamicRespondsTo(message);
      }

      public virtual bool UserDefined => false;

      IObject invokeMessage(IObject obj, Message message)
      {
         var messageName = message.Name;

         if (RespondsTo(messageName))
            if (messages.ContainsKey(messageName))
               return messages[messageName](obj, message);
            else
               return DynamicInvoke(obj, message);
         else
            throw messageNotFound(classOf(obj), messageName);
      }

      IObject invokeClassMessage(Message message)
      {
         var messageName = message.Name;

         if (ClassRespondsTo(messageName))
            if (classMessages.ContainsKey(messageName))
               return classMessages[messageName](this, message);
            else
               return ClassDynamicInvoke(message);
         else
            throw messageNotFound(this, messageName);
      }

      public IObject SendMessage(IObject obj, Message message)
      {
         registerIfUnregistered();
         return invokeMessage(obj, message);
      }

      public IObject SendMessage(IObject obj, string message, Arguments arguments)
      {
         return SendMessage(obj, new Message(message, arguments));
      }

      public IObject SendClassMessage(Message message)
      {
         registerClassIfUnregistered();
         return invokeClassMessage(message);
      }

      public IObject SendClassMessage(string message, Arguments arguments) => SendClassMessage(new Message(message, arguments));

      protected void numericMessages()
      {
         registerMessage("+",
            (obj, msg) => function(obj, msg, (x, y) => x + y, (x, y) => x + y, (x, y) => x + y, (x, y) => x.Add(y), "+"));
         registerMessage("-", (obj, msg) => function(obj, msg, (x, y) => x - y, (x, y) => x - y, (x, y) => x - y,
            (x, y) => x.Subtract(y), "-"));
         registerMessage("*", (obj, msg) => function(obj, msg, (x, y) => x * y, (x, y) => x * y, (x, y) => x * y,
            (x, y) => x.Multiply(y), "*"));
         registerMessage("/", (obj, msg) => function(obj, msg, (x, y) => x / y, (x, y) => x.Divide(y), "/"));
         registerMessage("//", integerDivision);
         registerMessage("%", (obj, msg) => function(obj, msg, (x, y) => x % y, (x, y) => x % y, (x, y) => x % y,
            (x, y) => x.Remainder(y), "%"));
         registerMessage("^", (obj, msg) => function(obj, msg, (x, y) => Math.Pow(x, y), (x, y) => x.Raise(y), "^"));
         registerMessage("atan2", (obj, msg) => function(obj, msg, (x, y) => Math.Atan2(y, x), (x, y) => x.Atan2(y), "atan2"));
         registerMessage("sign", (obj, msg) => function(obj, x => Math.Sign(x), x => Math.Sign(x), x => Math.Sign(x),
            x => (Int)x.Sign(), "sign"));
         registerMessage("sin", (obj, msg) => function(obj, x => Math.Sin(x), x => x.Sin()));
         registerMessage("cos", (obj, msg) => function(obj, x => Math.Cos(x), x => x.Cos()));
         registerMessage("tan", (obj, msg) => function(obj, x => Math.Tan(x), x => x.Tan()));
         registerMessage("asin", (obj, msg) => function(obj, x => Math.Asin(x), x => x.Asin()));
         registerMessage("acos", (obj, msg) => function(obj, x => Math.Acos(x), x => x.Acos()));
         registerMessage("atan", (obj, msg) => function(obj, x => Math.Atan(x), x => x.Atan()));
         registerMessage("sqrt", (obj, msg) => function(obj, x => Math.Sqrt(x), x => x.Sqrt()));
         registerMessage("log", (obj, msg) => function(obj, x => Math.Log10(x), x => x.Log()));
         registerMessage("ln", (obj, msg) => function(obj, x => Math.Log(x), x => x.Ln()));
         registerMessage("exp", (obj, msg) => function(obj, x => Math.Exp(x), x => x.Exp()));
         registerMessage("abs",
            (obj, msg) => function(obj, x => Math.Abs(x), x => Math.Abs(x), x => x, x => (Int)x.Abs(), "abs"));
         registerMessage("ceil", (obj, msg) => function(obj, x => x, x => Math.Ceiling(x), x => x, x => (Float)x.Ceiling(), "ceil"));
         registerMessage("floor", (obj, msg) => function(obj, x => x, x => Math.Floor(x), x => x, x => (Float)x.Floor(), "floor"));
         registerMessage("frac", (obj, msg) => function(obj, x => 0, x => x - (int)x, x => 0, x => (Float)x.Fraction(), "frac"));
      }

      protected void numericConversionMessages()
      {
         registerMessage("i".get(),
            (obj, msg) => function(obj, i => i, d => (int)d, b => b, m => (Int)((INumeric)m).ToInt(), "i".get()));
         registerMessage("f".get(),
            (obj, msg) => function(obj, i => i, d => d, b => b, m => (Float)((INumeric)m).ToFloat(), "f".get()));
         registerMessage("b".get(),
            (obj, msg) => function(obj, i => (byte)i, d => (byte)d, b => b, m => (Byte)((INumeric)m).ToByte(), "b".get()));
      }

      protected void formatMessage<T>() where T : IObject, IFormattable
      {
         registerMessage("format",
            (obj, msg) => function<T, String>(obj, msg, (v, s) => v is IFormattable f ? f.Format(s.Value) : String.Empty));
      }

      protected void collectionMessages()
      {
         registerMessage("getIterator", (obj, msg) => collectionFunc<Boolean>(obj, msg, (c, l) => (IObject)c.GetIterator(l.Value)));
         registerMessage("length".get(), (obj, msg) => collectionFunc(obj, c => c.Length));
         registerMessage("in", (obj, msg) => collectionFunc<IObject>(obj, msg, (c, i) => c.In(i)));
         registerMessage("notIn", (obj, msg) => collectionFunc<IObject>(obj, msg, (c, i) => c.NotIn(i)));
         registerMessage("*", (obj, msg) => collectionFunc<Int>(obj, msg, (c, i) => c.Times(i.Value)));
         registerMessage("flatten", (obj, msg) => collectionFunc(obj, c => c.Flatten()));

         loadIteratorMessages();
      }

      protected void mutableCollectionMessages()
      {
         registerMessage("<<", (obj, msg) => function<IObject, IObject>(obj, msg, (o, v) => ((IMutableCollection)o).Append(v)));
         registerMessage("append", (obj, msg) => function<IObject, IObject>(obj, msg, (o, v) => ((IMutableCollection)o).Append(v)));
         registerMessage(">>", (obj, msg) => function<IObject, IObject>(obj, msg, (o, v) => ((IMutableCollection)o).Remove(v)));
         registerMessage("remove", (obj, msg) => function<IObject, IObject>(obj, msg, (o, v) => ((IMutableCollection)o).Remove(v)));
         registerMessage("-", (obj, msg) => function<IObject, IObject>(obj, msg, (o, v) => ((IMutableCollection)o).Remove(v)));
         registerMessage("remove".Function("at"),
            (obj, msg) => function<IObject, Int>(obj, msg, (o, i) => ((IMutableCollection)o).RemoveAt(i.Value)));
         registerMessage("insert".Function("at", "value"),
            (obj, msg) => function<IObject, Int, IObject>(obj, msg, (o, i, v) => ((IMutableCollection)o).InsertAt(i.Value, v)));
      }

      void loadIteratorMessages()
      {
         alternateMessages.AddRange(array("collection".get(), "isLazy".get(), "next", "peek", "reverse", "join",
            "sort".Function("with", "asc"), "sort".Function("with"), "sort".Function("asc"), "sort", "foldl".Function("init", "with"),
            "foldl", "foldr".Function("init", "with"), "foldr", "reducel".Function("init", "with"), "reducel",
            "reducer".Function("init", "with"), "reducer", "count", "map", "if", "ifNot", "skip", "skip".Function("while"),
            "skip".Function("until"), "take", "take".Function("while"), "take".Function("until"), "index", "indexes",
            "zip".Function("on", "with"), "zip", "min".get(), "max".get(), "first", "first".Function("with"), "last",
            "last".Function("with"),
            "split", "split".Function("count"), "groupBy", "one", "none", "any", "all", "sum", "average", "product",
            "cross", "by", "distinct", "span".Function("with"), "span".Function("count"), "shuffle", "shuffle".Function("count"),
            "array", "list", "tuple", "dictionary".Function("key", "value"), "dictionary", "each", "rotate"));

         dynamicInvoke = (obj, msg) =>
         {
            var iterator = (IObject)((ICollection)obj).GetIterator(false);
            return classOf(iterator).SendMessage(iterator, msg);
         };
      }

      protected void iteratorMessages()
      {
         registerMessage("collection".get(), (obj, msg) => iteratorFunc(obj, i => (IObject)i.Collection));
         registerMessage("isLazy".get(), (obj, msg) => iteratorFunc(obj, i => (Boolean)i.IsLazy));
         registerMessage("next", (obj, msg) => iteratorFunc(obj, i => i.Next().FlatMap(s => new Some(s), () => Nil.NilValue)));
         registerMessage("peek", (obj, msg) => iteratorFunc(obj, i => i.Peek().FlatMap(s => new Some(s), () => Nil.NilValue)));
         registerMessage("reverse", (obj, msg) => iteratorFunc(obj, i => i.Reverse()));
         registerMessage("join", (obj, msg) => iteratorFunc<String>(obj, msg, (i, s) => i.Join(s.Value)));
         registerMessage("sort".Function("with", "asc"),
            (obj, msg) => iteratorFunc<Lambda, Boolean>(obj, msg, (i, l, b) => i.Sort(l, b.Value)));
         registerMessage("sort".Function("with"), (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.Sort(l, true)));
         registerMessage("sort".Function("asc"), (obj, msg) => iteratorFunc<Boolean>(obj, msg, (i, b) => i.Sort(b.Value)));
         registerMessage("sort", (obj, msg) => iteratorFunc(obj, i => i.Sort(true)));
         registerMessage("foldl".Function("init", "with"),
            (obj, msg) => iteratorFunc<IObject, Lambda>(obj, msg, (i, o, l) => i.FoldLeft(o, l)));
         registerMessage("foldl", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.FoldLeft(l)));
         registerMessage("foldr".Function("init", "with"),
            (obj, msg) => iteratorFunc<IObject, Lambda>(obj, msg, (i, o, l) => i.FoldRight(o, l)));
         registerMessage("foldr", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.FoldRight(l)));
         registerMessage("reducel".Function("init", "with"),
            (obj, msg) => iteratorFunc<IObject, Lambda>(obj, msg, (i, o, l) => i.ReduceLeft(o, l)));
         registerMessage("reducel", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.ReduceLeft(l)));
         registerMessage("reducer".Function("init", "with"),
            (obj, msg) => iteratorFunc<IObject, Lambda>(obj, msg, (i, o, l) => i.ReduceRight(o, l)));
         registerMessage("reducer", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.ReduceRight(l)));
         registerMessage("count", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.Count(l)));
         registerMessage("map", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.Map(l)));
         registerMessage("if", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.If(l)));
         registerMessage("ifNot", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.IfNot(l)));
         registerMessage("skip", (obj, msg) => iteratorFunc<Int>(obj, msg, (i, j) => i.Skip(j.Value)));
         registerMessage("skip".Function("while"), (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.SkipWhile(l)));
         registerMessage("skip".Function("until"), (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.SkipUntil(l)));
         registerMessage("take", (obj, msg) => iteratorFunc<Int>(obj, msg, (i, j) => i.Take(j.Value)));
         registerMessage("take".Function("while"), (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.TakeWhile(l)));
         registerMessage("take".Function("until"), (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.TakeUntil(l)));
         registerMessage("index", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.Index(l)));
         registerMessage("indexes", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.Indexes(l)));
         registerMessage("zip".Function("on", "with"),
            (obj, msg) => iteratorFunc<IObject, Lambda>(obj, msg, (i, c, l) => i.Zip((ICollection)c, l)));
         registerMessage("zip", (obj, msg) => iteratorFunc<IObject>(obj, msg, (i, c) => i.Zip((ICollection)c)));
         registerMessage("min".get(), (obj, msg) => iteratorFunc(obj, i => i.Min()));
         registerMessage("max".get(), (obj, msg) => iteratorFunc(obj, i => i.Max()));
         registerMessage("first", (obj, msg) => iteratorFunc(obj, i => i.First()));
         registerMessage("first".Function("with"), (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.First(l)));
         registerMessage("last", (obj, msg) => iteratorFunc(obj, i => i.Last()));
         registerMessage("last".Function("with"), (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.Last(l)));
         registerMessage("split", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.Split(l)));
         registerMessage("split".Function("count"), (obj, msg) => iteratorFunc<Int>(obj, msg, (i, j) => i.Split(j.Value)));
         registerMessage("groupBy", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.GroupBy(l)));
         registerMessage("one", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.One(l)));
         registerMessage("none", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.None(l)));
         registerMessage("any", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.Any(l)));
         registerMessage("all", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.All(l)));
         registerMessage("sum", (obj, msg) => iteratorFunc(obj, i => (IObject)i.Sum()));
         registerMessage("average", (obj, msg) => iteratorFunc(obj, i => (IObject)i.Average()));
         registerMessage("product", (obj, msg) => iteratorFunc(obj, i => (IObject)i.Product()));
         registerMessage("cross", (obj, msg) => iteratorFunc<IObject>(obj, msg, (i, c) => i.Cross((ICollection)c)));
         registerMessage("by", (obj, msg) => iteratorFunc<Int>(obj, msg, (i, j) => i.By(j.Value)));
         registerMessage("distinct", (obj, msg) => iteratorFunc(obj, i => i.Distinct()));
         registerMessage("span".Function("with"), (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.Span(l)));
         registerMessage("span".Function("count"), (obj, msg) => iteratorFunc<Int>(obj, msg, (i, j) => i.Span(j.Value)));
         registerMessage("shuffle", (obj, msg) => iteratorFunc(obj, i => i.Shuffle()));
         registerMessage("shuffle".Function("count"), (obj, msg) => iteratorFunc<Int>(obj, msg, (i, j) => i.Shuffle(j.Value)));
         registerMessage("array", (obj, msg) => iteratorFunc(obj, i => i.ToArray()));
         registerMessage("list", (obj, msg) => iteratorFunc(obj, i => i.ToList()));
         registerMessage("tuple", (obj, msg) => iteratorFunc(obj, i => i.ToTuple()));
         registerMessage("dictionary".Function("key", "value"), (obj, msg) => iteratorFunc<Lambda, Lambda>(obj, msg, (i, l1, l2) => i.ToDictionary(l1, l2)));
         registerMessage("dictionary", (obj, msg) => iteratorFunc(obj, i => i.ToDictionary()));
         registerMessage("each", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.Each(l)));
         registerMessage("rotate", (obj, msg) => iteratorFunc<Int>(obj, msg, (i, c) => i.Rotate(c.Value)));
      }

      public virtual bool MatchCompatible(BaseClass otherClass) => Name == otherClass.Name;

      public virtual bool AssignCompatible(BaseClass otherClass) => Name == otherClass.Name;

      protected void rangeMessages()
      {
         registerMessage("succ".get(), (obj, msg) => function<IObject>(obj, o => (IObject)((IRangeItem)o).Successor));
         registerMessage("pred".get(), (obj, msg) => function<IObject>(obj, o => (IObject)((IRangeItem)o).Predecessor));
         registerMessage("range", (obj, msg) => function<IObject>(obj, o => ((IRangeItem)o).Range()));
      }

      public static IObject Invoke(IObject obj, Arguments arguments, Lambda lambda)
      {
         Fields fields;
         if (obj is UserObject uo)
            fields = uo.Fields;
         else
         {
            fields = new Fields();
            fields.New("self");
            fields.Assign("self", obj);
         }

         if (Machine.Current.Invoke(lambda.Invokable, arguments, fields).If(out var value, out var isNotMatched, out var exception))
            return value;
         else if (isNotMatched)
            return Void.Value;
         else
            throw exception;
      }

      public static IObject Invoke(UserClass userClass, Arguments arguments, Lambda lambda)
      {
         return Machine.Current.Invoke(lambda.Invokable, arguments, userClass.ClassFields).Value;
      }

      protected void messageNumberMessages()
      {
         registerMessage("-(", (obj, msg) => msgNumberFunction(obj, mn => mn.Negate()));
         registerMessage("sign", (obj, msg) => msgNumberFunction(obj, mn => mn.Sign()));
         registerMessage("^", (obj, msg) => msgNumberFunction(obj, msg, (x, y) => x.Raise(y)));
         registerMessage("mod", (obj, msg) => msgNumberFunction(obj, msg, (x, y) => x.Remainder(y)));
         registerMessage("/", (obj, msg) => msgNumberFunction(obj, msg, (x, y) => x.Divide(y)));
         registerMessage("/%", (obj, msg) => msgNumberFunction(obj, msg, (x, y) => x.DivRem(y)));
         registerMessage("+", (obj, msg) => msgNumberFunction(obj, msg, (x, y) => x.Add(y)));
         registerMessage("-", (obj, msg) => msgNumberFunction(obj, msg, (x, y) => x.Subtract(y)));
         registerMessage("*", (obj, msg) => msgNumberFunction(obj, msg, (x, y) => x.Multiply(y)));
         registerMessage("sin", (obj, msg) => msgNumberFunction(obj, mn => mn.Sin()));
         registerMessage("cos", (obj, msg) => msgNumberFunction(obj, mn => mn.Cos()));
         registerMessage("tan", (obj, msg) => msgNumberFunction(obj, mn => mn.Tan()));
         registerMessage("asin", (obj, msg) => msgNumberFunction(obj, mn => mn.Asin()));
         registerMessage("acos", (obj, msg) => msgNumberFunction(obj, mn => mn.Acos()));
         registerMessage("atan", (obj, msg) => msgNumberFunction(obj, mn => mn.Atan()));
         registerMessage("sinh", (obj, msg) => msgNumberFunction(obj, mn => mn.Sinh()));
         registerMessage("cosh", (obj, msg) => msgNumberFunction(obj, mn => mn.Cosh()));
         registerMessage("tanh", (obj, msg) => msgNumberFunction(obj, mn => mn.Tanh()));
         registerMessage("asinh", (obj, msg) => msgNumberFunction(obj, mn => mn.Asinh()));
         registerMessage("acosh", (obj, msg) => msgNumberFunction(obj, mn => mn.Acosh()));
         registerMessage("atanh", (obj, msg) => msgNumberFunction(obj, mn => mn.Atanh()));
         registerMessage("atan2", (obj, msg) => msgNumberFunction(obj, msg, (x, y) => x.Atan2(y)));
         registerMessage("sqrt", (obj, msg) => msgNumberFunction(obj, mn => mn.Sqrt()));
         registerMessage("log", (obj, msg) => msgNumberFunction(obj, mn => mn.Log()));
         registerMessage("ln", (obj, msg) => msgNumberFunction(obj, mn => mn.Ln()));
         registerMessage("exp", (obj, msg) => msgNumberFunction(obj, mn => mn.Exp()));
         registerMessage("abs", (obj, msg) => msgNumberFunction(obj, mn => mn.Abs()));
         registerMessage("ceil", (obj, msg) => msgNumberFunction(obj, mn => mn.Ceiling()));
         registerMessage("floor", (obj, msg) => msgNumberFunction(obj, mn => mn.Floor()));
         registerMessage("frac", (obj, msg) => msgNumberFunction(obj, mn => mn.Fraction()));
         registerMessage("round", (obj, msg) => msgNumberFunction(obj, msg, (x, y) => x.Round(y)));
      }

      protected void sliceableMessages()
      {
         registerMessage("$",
            (obj, msg) => function<IObject, IObject>(obj, msg, (o1, o2) =>
            {
               var sliceable = (ISliceable)o1;
               switch (o2)
               {
                  case Range range:
                     if (range.StopObj is End)
                     {
                        var length = sliceable.Length;
                        var start = range.StartObj;
                        if (start is Int i && i.Value < 0)
                           start = (Int)wrapIndex(i.Value, length);
                        var newRange = new Range((IRangeItem)start, (Int)(length - 1), range.Inclusive, range.Increment);
                        return sliceable.Slice(newRange);
                     }
                     else
                        return sliceable.Slice(range);
                  case ICollection collection:
                     return sliceable.Slice(collection);
                  default:
                     var tuple = new Tuple(o2);
                     return sliceable.Slice(tuple);
               }
            }));
      }

      protected void compareMessages()
      {
         registerMessage("<>", (o, m) => (Int)((IObjectCompare)o).Compare(m.Arguments[0]));
         registerMessage("between".Function("", "and"), (o, m) => ((IObjectCompare)o).Between(m.Arguments[0], m.Arguments[1], true));
         registerMessage("between".Function("", "until"), (o, m) => ((IObjectCompare)o).Between(m.Arguments[0], m.Arguments[1], false));
      }
   }
}