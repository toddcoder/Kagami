using System;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
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
      protected SelectorHash<Func<IObject, Message, IObject>> messages;
      protected SelectorHash<Func<BaseClass, Message, IObject>> classMessages;
      protected bool registered;
      protected bool classRegistered;
      protected SelectorSet alternateMessages;
      protected Func<IObject, Message, IObject> dynamicInvoke;
      protected Func<Message, IObject> classDynamicInvoke;
      protected Fields classFields;

      public BaseClass()
      {
         messages = new SelectorHash<Func<IObject, Message, IObject>>();
         classMessages = new SelectorHash<Func<BaseClass, Message, IObject>>();
         registered = false;
         classRegistered = false;
         alternateMessages = new SelectorSet();
         dynamicInvoke = (obj, msg) => throw messageNotFound(classOf(obj), msg.Selector);
         classDynamicInvoke = msg => throw messageNotFound(this, msg.Selector);
         classFields = new Fields();
      }

      public abstract string Name { get; }

      public Fields ClassFields => classFields;

      public virtual IObject DynamicInvoke(IObject obj, Message message) => dynamicInvoke(obj, message);

      public virtual IObject ClassDynamicInvoke(Message message) => classDynamicInvoke(message);

      public virtual bool DynamicRespondsTo(Selector selector) => alternateMessages.Contains(selector);

      public virtual bool ClassDynamicRespondsTo(Selector selector) => false;

      protected void registerMessage(Selector selector, Func<IObject, Message, IObject> function)
      {
         if (!messages.ContainsKey(selector))
            messages[selector] = function;
      }

      protected void registerClassMessage(Selector selector, Func<BaseClass, Message, IObject> function)
      {
         if (!classMessages.ContainsKey(selector))
            classMessages[selector] = function;
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
         messages["isNumber".get()] = (obj, msg) => Boolean.False;
         registerMessage("send",
            (obj, msg) => function<IObject, String>(obj, msg, (o, n) => sendMessage(o, n.Value, msg.Arguments.Pass(1))));
      }

      public virtual void RegisterClassMessages()
      {
         registerClassMessage("name".get(), (cls, msg) => String.StringObject(Name));
      }

      public virtual void RegisterMessage(Selector selector, Func<IObject, Message, IObject> func) => messages[selector] = func;

      public void RegisterClassMessage(Selector selector, Func<BaseClass, Message, IObject> func) => classMessages[selector] = func;

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

      public virtual bool RespondsTo(Selector selector)
      {
         registerIfUnregistered();

         return messages.ContainsKey(selector) || DynamicRespondsTo(selector);
      }

      public virtual bool ClassRespondsTo(Selector selector)
      {
         registerClassIfUnregistered();

         return classMessages.ContainsKey(selector) || ClassDynamicRespondsTo(selector);
      }

      public virtual bool UserDefined => false;

      IObject invokeMessage(IObject obj, Message message)
      {
         var selector = message.Selector;

         if (RespondsTo(selector))
            if (messages.ContainsKey(selector))
               return messages[selector](obj, message);
            else
               return DynamicInvoke(obj, message);
         else
            throw messageNotFound(classOf(obj), selector);
      }

      IObject invokeClassMessage(Message message)
      {
         var selector = message.Selector;

         if (ClassRespondsTo(selector))
            if (classMessages.ContainsKey(selector))
               return classMessages[selector](this, message);
            else
               return ClassDynamicInvoke(message);
         else
            throw messageNotFound(this, selector);
      }

      public IObject SendMessage(IObject obj, Message message)
      {
         registerIfUnregistered();
         return invokeMessage(obj, message);
      }

      public IObject SendMessage(IObject obj, Selector selector, Arguments arguments)
      {
         return SendMessage(obj, new Message(selector, arguments));
      }

      public IObject SendClassMessage(Message message)
      {
         registerClassIfUnregistered();
         return invokeClassMessage(message);
      }

      public IObject SendClassMessage(Selector selector, Arguments arguments) => SendClassMessage(new Message(selector, arguments));

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
         messages["isNumber".get()] = (obj, msg) => Boolean.True;
         registerMessage("isZero".get(), (obj, msg) => function(obj, numeric => (Boolean)numeric.IsZero));
         registerMessage("isPositive".get(), (obj, msg) => function(obj, numeric => (Boolean)numeric.IsPositive));
         registerMessage("isNegative".get(), (obj, msg) => function(obj, numeric => (Boolean)numeric.IsNegative));
         registerMessage("isPrimitive".get(), (obj, msg) => function(obj, numeric => (Boolean)numeric.IsPrimitive));
         registerMessage("zfill", (obj, msg) => function<IObject, Int>(obj, msg, (numeric, i) => ((INumeric)numeric).ZFill(i.Value)));
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
         registerMessage("indexed", (obj, msg) => collectionFunc(obj, c => (IObject)c.GetIndexedIterator()));

         loadIteratorMessages();
      }

      protected void mutableCollectionMessages()
      {
         registerMessage("<<", (obj, msg) => function<IObject, IObject>(obj, msg, (o, v) => ((IMutableCollection)o).Append(v)));
         registerMessage("append", (obj, msg) => function<IObject, IObject>(obj, msg, (o, v) => ((IMutableCollection)o).Append(v)));
         registerMessage(">>", (obj, msg) => function<IObject, IObject>(obj, msg, (o, v) => ((IMutableCollection)o).Remove(v)));
         registerMessage("remove", (obj, msg) => function<IObject, IObject>(obj, msg, (o, v) => ((IMutableCollection)o).Remove(v)));
         registerMessage("-", (obj, msg) => function<IObject, IObject>(obj, msg, (o, v) => ((IMutableCollection)o).Remove(v)));
         registerMessage("remove".Selector("at:<Int>"),
            (obj, msg) => function<IObject, Int>(obj, msg, (o, i) => ((IMutableCollection)o).RemoveAt(i.Value)));
         registerMessage("insert".Selector("at:<Int>", "value:"),
            (obj, msg) => function<IObject, Int, IObject>(obj, msg, (o, i, v) => ((IMutableCollection)o).InsertAt(i.Value, v)));
      }

      void loadIteratorMessages()
      {
         alternateMessages.AddRange(array<string>("collection".get(), "isLazy".get(), "next", "peek", "reverse", "join",
            "sort".Selector("<Lambda>", "asc:<Boolean>"), "sort".Selector("<Lambda>"), "sort".Selector("<Boolean>"), "sort", "foldl".Selector("", "<Lambda>"),
            "foldl", "foldr".Selector("", "<Lambda>"), "foldr", "reducel".Selector("", "<Lambda>"), "reducel",
            "reducer".Selector("", "<Lambda>"), "reducer", "count", "map", "if", "ifNot", "skip", "skip".Selector("while:<Lambda>"),
            "skip".Selector("until:<Lambda>"), "take", "take".Selector("while:<Lambda>"), "take".Selector("until:<Lambda>"), "index", "indexes",
            "zip".Selector("<Collection>", "<Lambda>"), "zip", "min".get(), "min", "max".get(), "max", "first", "first".Selector("<Lambda>"), "last",
            "last".Selector("<Lambda>"),
            "split", "split".Selector("<Int>"), "random", "group".Selector("by:<Lambda>"), "one", "none", "any", "all", "sum", "average",
            "product", "cross", "by", "window", "distinct", "span".Selector("<Lambda>"), "span".Selector("<Int>"), "shuffle",
            "array", "list", "tuple", "dictionary".Selector("key:<Lambda>", "value:<Lambda>"), "dictionary", "each", "rotate", "permutation",
            "combination", "flatten",
            "copy", "revert"));

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
         registerMessage("sort".Selector("<Lambda>", "asc:<Boolean>"),
            (obj, msg) => iteratorFunc<Lambda, Boolean>(obj, msg, (i, l, b) => i.Sort(l, b.Value)));
         registerMessage("sort".Selector("<Lambda>"), (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.Sort(l, true)));
         registerMessage("sort".Selector("<Boolean>"), (obj, msg) => iteratorFunc<Boolean>(obj, msg, (i, b) => i.Sort(b.Value)));
         registerMessage("sort", (obj, msg) => iteratorFunc(obj, i => i.Sort(true)));
         registerMessage("foldl".Selector("", "<Lambda>"),
            (obj, msg) => iteratorFunc<IObject, Lambda>(obj, msg, (i, o, l) => i.FoldLeft(o, l)));
         registerMessage("foldl", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.FoldLeft(l)));
         registerMessage("foldr".Selector("", "<Lambda>"),
            (obj, msg) => iteratorFunc<IObject, Lambda>(obj, msg, (i, o, l) => i.FoldRight(o, l)));
         registerMessage("foldr", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.FoldRight(l)));
         registerMessage("reducel".Selector("", "<Lambda>"),
            (obj, msg) => iteratorFunc<IObject, Lambda>(obj, msg, (i, o, l) => i.ReduceLeft(o, l)));
         registerMessage("reducel", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.ReduceLeft(l)));
         registerMessage("reducer".Selector("", "with"),
            (obj, msg) => iteratorFunc<IObject, Lambda>(obj, msg, (i, o, l) => i.ReduceRight(o, l)));
         registerMessage("reducer", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.ReduceRight(l)));
         registerMessage("count", (obj, msg) => iteratorFunc<IObject>(obj, msg, (i, o) => i.Count(o)));
         registerMessage("count".Selector("<Lambda>"), (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.Count(l)));
         registerMessage("map", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.Map(l)));
         registerMessage("if", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.If(l)));
         registerMessage("ifNot", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.IfNot(l)));
         registerMessage("skip", (obj, msg) => iteratorFunc<Int>(obj, msg, (i, j) => i.Skip(j.Value)));
         registerMessage("skip".Selector("while:<Lambda>"), (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.SkipWhile(l)));
         registerMessage("skip".Selector("until:<Lambda>"), (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.SkipUntil(l)));
         registerMessage("take", (obj, msg) => iteratorFunc<Int>(obj, msg, (i, j) => i.Take(j.Value)));
         registerMessage("take".Selector("while:<Lambda>"), (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.TakeWhile(l)));
         registerMessage("take".Selector("until:<Lambda>"), (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.TakeUntil(l)));
         registerMessage("index", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.Index(l)));
         registerMessage("indexes", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.Indexes(l)));
         registerMessage("zip".Selector("<Collection>", "<Lambda>"),
            (obj, msg) => iteratorFunc<IObject, Lambda>(obj, msg, (i, c, l) => i.Zip((ICollection)c, l)));
         registerMessage("zip", (obj, msg) => iteratorFunc<IObject>(obj, msg, (i, c) => i.Zip((ICollection)c)));
         registerMessage("min".get(), (obj, msg) => iteratorFunc(obj, i => i.Min()));
         registerMessage("min", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.Min(l)));
         registerMessage("max".get(), (obj, msg) => iteratorFunc(obj, i => i.Max()));
         registerMessage("max", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.Max(l)));
         registerMessage("first", (obj, msg) => iteratorFunc(obj, i => i.First()));
         registerMessage("first".Selector("<Lambda>"), (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.First(l)));
         registerMessage("last", (obj, msg) => iteratorFunc(obj, i => i.Last()));
         registerMessage("last".Selector("<Lambda>"), (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.Last(l)));
         registerMessage("split", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.Split(l)));
         registerMessage("split".Selector("<Int>"), (obj, msg) => iteratorFunc<Int>(obj, msg, (i, j) => i.Split(j.Value)));
         registerMessage("group".Selector("by:<Lambda>"), (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.GroupBy(l)));
         registerMessage("one", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.One(l)));
         registerMessage("none", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.None(l)));
         registerMessage("any", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.Any(l)));
         registerMessage("all", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.All(l)));
         registerMessage("sum", (obj, msg) => iteratorFunc(obj, i => (IObject)i.Sum()));
         registerMessage("average", (obj, msg) => iteratorFunc(obj, i => (IObject)i.Average()));
         registerMessage("product", (obj, msg) => iteratorFunc(obj, i => (IObject)i.Product()));
         registerMessage("cross", (obj, msg) => iteratorFunc<IObject>(obj, msg, (i, c) => i.Cross((ICollection)c)));
         registerMessage("by", (obj, msg) => iteratorFunc<Int>(obj, msg, (i, j) => i.By(j.Value)));
         registerMessage("window", (obj, msg) => iteratorFunc<Int>(obj, msg, (i, j) => i.Window(j.Value)));
         registerMessage("distinct", (obj, msg) => iteratorFunc(obj, i => i.Distinct()));
         registerMessage("span".Selector("<Lambda>"), (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.Span(l)));
         registerMessage("span".Selector("<Int>"), (obj, msg) => iteratorFunc<Int>(obj, msg, (i, j) => i.Span(j.Value)));
         registerMessage("shuffle", (obj, msg) => iteratorFunc(obj, i => i.Shuffle()));
         registerMessage("random", (obj, msg) => iteratorFunc(obj, i => i.Random()));
         registerMessage("array", (obj, msg) => iteratorFunc(obj, i => i.ToArray()));
         registerMessage("list", (obj, msg) => iteratorFunc(obj, i => i.ToList()));
         registerMessage("tuple", (obj, msg) => iteratorFunc(obj, i => i.ToTuple()));
         registerMessage("dictionary".Selector("key:<Lambda>", "value:<Lambda>"),
            (obj, msg) => iteratorFunc<Lambda, Lambda>(obj, msg, (i, l1, l2) => i.ToDictionary(l1, l2)));
         registerMessage("dictionary", (obj, msg) => iteratorFunc(obj, i => i.ToDictionary()));
         registerMessage("each", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.Each(l)));
         registerMessage("rotate", (obj, msg) => iteratorFunc<Int>(obj, msg, (i, c) => i.Rotate(c.Value)));
         registerMessage("permutation", (obj, msg) => iteratorFunc<Int>(obj, msg, (i, c) => i.Permutation(c.Value)));
         registerMessage("combination", (obj, msg) => iteratorFunc<Int>(obj, msg, (i, c) => i.Combination(c.Value)));
         registerMessage("flatten", (obj, msg) => iteratorFunc(obj, i => i.Flatten()));
         registerMessage("copy", (obj, msg) => iteratorFunc(obj, i => i.Copy()));
         registerMessage("collect", (obj, msg) => iteratorFunc(obj, i => i.Collect()));
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
         registerMessage("between".Selector("", "and:"), (o, m) => ((IObjectCompare)o).Between(m.Arguments[0], m.Arguments[1], true));
         registerMessage("between".Selector("", "until:"),
            (o, m) => ((IObjectCompare)o).Between(m.Arguments[0], m.Arguments[1], false));
         registerMessage("after".Selector("", "and:"), (o, m) => ((IObjectCompare)o).After(m.Arguments[0], m.Arguments[1], true));
         registerMessage("after".Selector("", "until:"),
            (o, m) => ((IObjectCompare)o).After(m.Arguments[0], m.Arguments[1], false));
      }

      public virtual bool IsNumeric => false;

      protected void textFindingMessages()
      {
         IObject apply(IObject obj, Message msg, Func<String, ITextFinding, IObject> func)
         {
            var input = (String)obj;
            var textFinding = (ITextFinding)msg.Arguments[0];

            return func(input, textFinding);
         }

         IObject apply1<T>(IObject obj, Message msg, Func<String, ITextFinding, T, IObject> func)
            where T : IObject
         {
            var input = (String)obj;
            var textFinding = (ITextFinding)msg.Arguments[0];
            var arg1 = (T)msg.Arguments[1];

            return func(input, textFinding, arg1);
         }

         IObject apply2<T1, T2>(IObject obj, Message msg, Func<String, ITextFinding, T1, T2, IObject> func)
            where T1 : IObject
            where T2 : IObject
         {
            var input = (String)obj;
            var textFinding = (ITextFinding)msg.Arguments[0];
            var arg1 = (T1)msg.Arguments[1];
            var arg2 = (T2)msg.Arguments[2];

            return func(input, textFinding, arg1, arg2);
         }

         registerMessage("find", (obj, msg) => apply(obj, msg, (s, tf) => s.Find(tf, 0, false)));
         registerMessage("find".Selector("<TextFinding>", "startAt:<Int>"),
            (obj, msg) => apply1<Int>(obj, msg, (s, tf, i) => s.Find(tf, i.Value, false)));
         registerMessage("find".Selector("<TextFinding>", "reverse:<Boolean>"),
            (obj, msg) => apply1<Boolean>(obj, msg, (s, tf, b) => s.Find(tf, 0, b.Value)));
         registerMessage("find".Selector("<TextFinding>", "startAt:<Boolean>", "reverse:<Boolean>"),
            (obj, msg) => apply2<Int, Boolean>(obj, msg, (s, tf, i, b) => s.Find(tf, i.Value, b.Value)));
         registerMessage("find".Selector("all:<TextFinding>"), (obj, msg) => apply(obj, msg, (s, tf) => s.FindAll(tf)));
         registerMessage("replace".Selector("<TextFinding>", "new:<String>"), (obj, msg) => apply1<String>(obj, msg, (s1, tf, s2) => s1.Replace(tf, s2.Value, false)));
         registerMessage("replace".Selector("<TextFinding>", "new:<String>", "reverse:<Boolean>"),
            (obj, msg) => apply2<String, Boolean>(obj, msg, (s1, tf, s2, b) => s1.Replace(tf, s2.Value, b.Value)));
         registerMessage("replace".Selector("all:<TextFinding>", "new"),
            (obj, msg) => apply1<String>(obj, msg, (s1, tf, s2) => s1.ReplaceAll(tf, s2.Value)));
         registerMessage("replace".Selector("<TextFinding>", "with:<Lambda>"), (obj, msg) => apply1<Lambda>(obj, msg, (s, tf, l) => s.Replace(tf, l, false)));
         registerMessage("replace".Selector("<TextFinding>", "with:<Lambda>", "reverse:<Boolean>"),
            (obj, msg) => apply2<Lambda, Boolean>(obj, msg, (s, tf, l, b) => s.Replace(tf, l, b.Value)));
         registerMessage("replace".Selector("all:<TextFinding>", "with:<Lambda>"),
            (obj, msg) => apply1<Lambda>(obj, msg, (s, tf, l) => s.ReplaceAll(tf, l)));
         registerMessage("split".Selector("on:<TextFinding>"), (obj, msg) => apply(obj, msg, (s, tf) => s.Split(tf)));
         registerMessage("partition", (obj, msg) => apply(obj, msg, (s, tf) => s.Partition(tf, false)));
         registerMessage("partition".Selector("<TextFinding>", "reverse:<Boolean>"), (obj, msg) => apply1<Boolean>(obj, msg, (s, tf, b) => s.Partition(tf, b.Value)));
         registerMessage("count".Selector("<String>"), (obj, msg) => apply(obj, msg, (s, tf) => s.Count(tf)));
         registerMessage("count".Selector("<String>", "<Lambda>"), (obj, msg) => apply1<Lambda>(obj, msg, (s, tf, l) => s.Count(tf, l)));
      }
   }
}