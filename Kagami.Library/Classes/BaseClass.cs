using Core.Collections;
using Core.Objects;
using Kagami.Library.Inclusions;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Classes.ClassFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Operations.NumericFunctions;
using IFormattable = Kagami.Library.Objects.IFormattable;

namespace Kagami.Library.Classes;

public abstract class BaseClass
{
   protected SelectorHash<Func<IObject, Message, IObject>> messages = new();
   protected SelectorHash<Func<BaseClass, Message, IObject>> classMessages = new();
   protected bool registered;
   protected bool classRegistered;
   protected SelectorSet alternateMessages = [];
   protected Func<IObject, Message, IObject> dynamicInvoke;
   protected Func<Message, IObject> classDynamicInvoke;
   protected Fields classFields = new();
   protected StringHash<Inclusion> inclusions = [];

   public BaseClass()
   {
      dynamicInvoke = (obj, message) => throw messageNotFound(classOf(obj), message.Selector);
      classDynamicInvoke = message => throw messageNotFound(this, message.Selector);
   }

   public abstract string Name { get; }

   public Fields ClassFields => classFields;

   public virtual IObject DynamicInvoke(IObject obj, Message message) => dynamicInvoke(obj, message);

   public virtual IObject ClassDynamicInvoke(Message message) => classDynamicInvoke(message);

   public virtual bool DynamicRespondsTo(Selector selector) => alternateMessages.Contains(selector);

   // ReSharper disable once UnusedParameter.Global
   public virtual bool ClassDynamicRespondsTo(Selector selector) => false;

   protected void registerMessage(Selector selector, Func<IObject, Message, IObject> function)
   {
      if (!messages.ContainsKey(selector))
      {
         messages[selector] = function;
      }
   }

   protected void registerClassMessage(Selector selector, Func<BaseClass, Message, IObject> function)
   {
      if (!classMessages.ContainsKey(selector))
      {
         classMessages[selector] = function;
      }
   }

   public virtual void RegisterMessages()
   {
      registerMessage("string".get(), (obj, _) => KString.StringObject(obj.AsString));
      registerMessage("image".get(), (obj, _) => KString.StringObject(obj.Image));
      registerMessage("hash".get(), (obj, _) => Int.IntObject(obj.Hash));
      registerMessage("equals(_)", (obj, message) => KBoolean.BooleanObject(obj.IsEqualTo(message.Arguments[0])));
      registerMessage("className".get(), (obj, _) => KString.StringObject(obj.ClassName));
      registerMessage("class".get(), (obj, _) => new Class(obj.ClassName));
      registerMessage("match(_)", match);
      messages["isNumber".get()] = (_, _) => KBoolean.False;
      registerMessage("send(_,_...)",
         (obj, message) => function<IObject, Selector>(obj, message, (o, n) => sendMessage(o, n, message.Arguments.Pass(1))));
      registerMessage("send(_)",
         (obj, message) => function<IObject, Selector>(obj, message, (o, n) => sendMessage(o, n, Arguments.Empty)));
      registerMessage("receives(_)", (obj, message) => (KBoolean)classOf(obj).RespondsTo((Selector)message.Arguments[0]));
      registerMessage("seq(_)", (obj, message) => new OpenRange(obj, (Lambda)message.Arguments[0]));
      registerMessage("format(_)", (obj, message) => format(obj, message.Arguments[0].AsString));
      registerMessage("id".get(), (obj, _) => KString.StringObject(obj.Id.ToString()));
   }

   protected static KString format(IObject obj, string formattingString)
   {
      if (formattingString == "i")
      {
         return obj.Image;
      }
      else if (obj is IFormattable formattable)
      {
         return formattable.Format(formattingString);
      }
      else
      {
         return obj.AsString;
      }
   }

   public virtual void RegisterClassMessages()
   {
      registerClassMessage("name".get(), (_, _) => KString.StringObject(Name));
      registerClassMessage("includes(_<String>)", (_, message) => (KBoolean)inclusions.ContainsKey(message.Arguments[0].AsString));
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

   protected IObject invokeMessage(IObject obj, Message message)
   {
      var selector = message.Selector;

      if (RespondsTo(selector))
      {
         var func = messages[selector];
         // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
         if (func is not null)
         {
            return func(obj, message);
         }
         else
         {
            return DynamicInvoke(obj, message);
         }
         /*if (messages.Maybe[selector] is (true, var func))
         {
            return func(obj, message);
         }
         else
         {
            return DynamicInvoke(obj, message);
         }*/
      }
      else
      {
         return DynamicInvoke(obj, message); //throw messageNotFound(classOf(obj), selector);
      }
   }

   protected IObject invokeDirectly(IObject obj, Message message)
   {
      var result = messages[message.Selector];
      return result(obj, message);
   }

   protected IObject invokeClassMessage(Message message)
   {
      var selector = message.Selector;

      if (ClassRespondsTo(selector))
      {
         return classMessages.ContainsKey(selector) ? classMessages[selector](this, message) : ClassDynamicInvoke(message);
      }
      else
      {
         throw messageNotFound(this, selector);
      }
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
      registerMessage("+(_)",
         (obj, message) => function(obj, message, (x, y) => x + y, (x, y) => x + y, (x, y) => x + y, (x, y) => x.Add(y), "+"));
      registerMessage("-(_)", (obj, message) => function(obj, message, (x, y) => x - y, (x, y) => x - y, (x, y) => x - y,
         (x, y) => x.Subtract(y), "-"));
      registerMessage("*(_)", (obj, message) => function(obj, message, (x, y) => x * y, (x, y) => x * y, (x, y) => x * y,
         (x, y) => x.Multiply(y), "*"));
      registerMessage("/(_)", (obj, message) => function(obj, message, (x, y) => x / y, (x, y) => x.Divide(y), "/"));
      registerMessage("div(_)", integerDivision);
      registerMessage("%(_)", (obj, message) => function(obj, message, (x, y) => x % y, (x, y) => x % y, (x, y) => x % y,
         (x, y) => x.Remainder(y), "%"));
      registerMessage("^(_)", (obj, message) => function(obj, message, (x, y) => Math.Pow(x, y), (x, y) => x.Raise(y), "^"));
      registerMessage("atan2(_)", (obj, message) => function(obj, message, (x, y) => Math.Atan2(y, x), (x, y) => x.Atan2(y), "atan2"));
      registerMessage("sign()", (obj, _) => function(obj, x => Math.Sign(x), x => Math.Sign(x), x => Math.Sign(x),
         x => (Int)x.Sign(), "sign()"));
      registerMessage("sin()", (obj, _) => function(obj, x => Math.Sin(x), x => x.Sin()));
      registerMessage("cos()", (obj, _) => function(obj, x => Math.Cos(x), x => x.Cos()));
      registerMessage("tan()", (obj, _) => function(obj, x => Math.Tan(x), x => x.Tan()));
      registerMessage("asin()", (obj, _) => function(obj, x => Math.Asin(x), x => x.Asin()));
      registerMessage("acos()", (obj, _) => function(obj, x => Math.Acos(x), x => x.Acos()));
      registerMessage("atan()", (obj, _) => function(obj, x => Math.Atan(x), x => x.Atan()));
      registerMessage("sqrt()", (obj, _) => function(obj, x => Math.Sqrt(x), x => x.Sqrt()));
      registerMessage("log()", (obj, _) => function(obj, x => Math.Log10(x), x => x.Log()));
      registerMessage("ln()", (obj, _) => function(obj, x => Math.Log(x), x => x.Ln()));
      registerMessage("exp()", (obj, _) => function(obj, x => Math.Exp(x), x => x.Exp()));
      registerMessage("abs()",
         (obj, _) => function(obj, x => Math.Abs(x), x => Math.Abs(x), x => x, x => (Int)x.Abs(), "abs()"));
      registerMessage("ceil()",
         (obj, _) => function(obj, x => x, x => Math.Ceiling(x), x => x, x => (Float)x.Ceiling(), "ceil()"));
      registerMessage("floor()", (obj, _) => function(obj, x => x, x => Math.Floor(x), x => x, x => (Float)x.Floor(), "floor()"));
      registerMessage("frac()", (obj, _) => function(obj, _ => 0, x => x - (int)x, _ => 0, x => (Float)x.Fraction(), "frac()"));
      messages["isNumber".get()] = (_, _) => KBoolean.True;
      registerMessage("isZero".get(), (obj, _) => function(obj, numeric => (KBoolean)numeric.IsZero));
      registerMessage("isPositive".get(), (obj, _) => function(obj, numeric => (KBoolean)numeric.IsPositive));
      registerMessage("isNegative".get(), (obj, _) => function(obj, numeric => (KBoolean)numeric.IsNegative));
      registerMessage("isPrimitive".get(), (obj, _) => function(obj, numeric => (KBoolean)numeric.IsPrimitive));
      registerMessage("zfill(_<Int>)",
         (obj, message) => function<IObject, Int>(obj, message, (numeric, i) => ((INumeric)numeric).ZFill(i.Value)));
   }

   protected void numericConversionMessages()
   {
      registerMessage("i".get(),
         (obj, _) => function(obj, i => i, d => (int)d, b => b, m => (Int)((INumeric)m).ToInt(), "i".get()));
      registerMessage("f".get(),
         (obj, _) => function(obj, i => i, d => d, b => b, m => (Float)((INumeric)m).ToFloat(), "f".get()));
      registerMessage("b".get(),
         (obj, _) => function(obj, i => (byte)i, d => (byte)d, b => b, m => (KByte)((INumeric)m).ToByte(), "b".get()));
      registerMessage("d".get(), (obj, _) => function(obj, i => i, d => (decimal)d, b => b, m => (XDecimal)((INumeric)m).ToDecimal(), "d".get()));
   }

   protected void collectionMessages()
   {
      registerMessage("getIterator(_<Boolean>)",
         (obj, message) => collectionFunc<KBoolean>(obj, message, (c, l) => (IObject)c.GetIterator(l.Value)));
      registerMessage("length".get(), (obj, _) => collectionFunc(obj, c => c.Length));
      registerMessage("in(_)", (obj, message) => collectionFunc<IObject>(obj, message, (c, i) => c.In(i)));
      registerMessage("notIn(_)", (obj, message) => collectionFunc<IObject>(obj, message, (c, i) => c.NotIn(i)));
      registerMessage("*(_<Int>)", (obj, message) => collectionFunc<Int>(obj, message, (c, i) => c.Times(i.Value)));
      registerMessage("*(_<String>)",
         (obj, message) => collectionFunc<KString>(obj, message, (c, connector) => c.MakeString(connector.Value)));
      registerMessage("indexed()", (obj, _) => collectionFunc(obj, c => (IObject)c.GetIndexedIterator()));
      registerMessage("[](_<SkipTake>)", (obj, message) => ((ISkipTakeable)obj)[(SkipTake)message.Arguments[0]]);
      registerMessage("range()", (obj, _) => collectionFunc(obj, c => new KRange(new Int(0), c.Length, false)));

      loadIteratorMessages();
   }

   protected void mutableCollectionMessages()
   {
      registerMessage("<<(_)", (obj, message) => function<IObject, IObject>(obj, message, (o, v) => ((IMutableCollection)o).Append(v)));
      registerMessage("append(_)", (obj, message) => function<IObject, IObject>(obj, message, (o, v) => ((IMutableCollection)o).Append(v)));
      registerMessage(">>(_)", (obj, message) => function<IObject, IObject>(obj, message, (o, v) => ((IMutableCollection)o).Remove(v)));
      registerMessage("remove(_)", (obj, message) => function<IObject, IObject>(obj, message, (o, v) => ((IMutableCollection)o).Remove(v)));
      registerMessage("-(_)", (obj, message) => function<IObject, IObject>(obj, message, (o, v) => ((IMutableCollection)o).Remove(v)));
      registerMessage("remove(at:_<Int>)",
         (obj, message) => function<IObject, Int>(obj, message, (o, i) => ((IMutableCollection)o).RemoveAt(i.Value)));
      registerMessage("remove(all:_)",
         (obj, message) => function<IObject, IObject>(obj, message, (o, v) => ((IMutableCollection)o).RemoveAll(v)));
      registerMessage("insert(at:_<Int>,value:_)",
         (obj, message) => function<IObject, Int, IObject>(obj, message, (o, i, v) => ((IMutableCollection)o).InsertAt(i.Value, v)));
      registerMessage("isEmpty".get(), (obj, _) => function<IObject>(obj, o => ((IMutableCollection)o).IsEmpty));
      registerMessage("assign(_,_)", (obj, message) => function<IObject, IObject, IObject>(obj, message, assignToMutable));
   }

   protected void loadIteratorMessages()
   {
      alternateMessages.AddRange((Selector[])
      [
         "collection".get(), "isLazy".get(), "next()", "peek()", "reset()", "reverse()",
         "join(_<String>)",
         "sort(_<Lambda>,asc:_<Boolean>)", "sort(_<Lambda>)", "sort(_<Boolean>)", "sort()",
         "foldl".Selector("_", "_<Lambda>"),
         "foldl(_)", "foldr".Selector("_", "_<Lambda>"), "foldr(_)", "reducel".Selector("_", "_<Lambda>"), "reducel(_)",
         "reducer".Selector("_", "_<Lambda>"), "reducer(_)", "count(of:_)", "count(_<Lambda>)", "map(_<Lambda>)",
         "flatMap(_<Lambda>)", "bind(_<Lambda>)",
         "if(_<Lambda>)",
         "ifNot(_<Lambda>)", "skip(_<Int>)", "-(_<Int>)", "skipWhile(_<Lambda>)",
         "skipUntil(_<Lambda>)", "take(_<Int>)", "+(_<Int>)", "takeWhile(_<Lambda>)",
         "takeUntil(_<Lambda>)",
         "index(_<Lambda>)", "indexes(_<Lambda>)",
         "zip(_<Collection>,_<Lambda>)", "zip(_)", "min".get(), "min(_<Lambda>)", "max".get(), "max(_<Lambda>)",
         "first()", "first".Selector("_<Lambda>"), "last()", "last".Selector("_<Lambda>"),
         "split(_<Lambda>)", "split".Selector("_<Int>"), "random()", "groupBy(_<Lambda>)", "one(_<Lambda>)",
         "none(_<Lambda>)",
         "any(_<Lambda>)", "all(_<Lambda>)", "sum()", "average()",
         "product()", "cross(_)", "cross(_,_)", "by(_<Int>)", "/(_<Int>)", "window(_<Int>)", "//(_<Int>)", "distinct()",
         "span".Selector("_<Lambda>"),
         "span".Selector("_<Int>"),
         "shuffle()", "array()", "list()", "tuple()", "dictionary".Selector("key:_<Lambda>", "value:_<Lambda>"), "dictionary()",
         "each(_<Lambda>)", "rotate(_<Int>)", "permutation(_<Int>)", "combination(_<Int>)", "flatten()",
         "copy()", "revert()", "*(_)", "format(_)", "replace(_<Lambda>,_<Lambda>)", "set()", "shape(_<Int>,_<Int>)",
         "column(_<Int>)", "step(_<Int>)"
      ]);

      dynamicInvoke = (obj, message) =>
      {
         var iterator = (IObject)((ICollection)obj).GetIterator(false);
         return classOf(iterator).SendMessage(iterator, message);
      };
   }

   protected void iteratorMessages()
   {
      registerMessage("collection".get(), (obj, _) => iteratorFunc(obj, i => (IObject)i.Collection));
      registerMessage("isLazy".get(), (obj, _) => iteratorFunc(obj, i => (KBoolean)i.IsLazy));
      registerMessage("next()", (obj, _) => iteratorFunc(obj, i => i.Next().Map(Some.Object) | (() => None.NoneValue)));
      registerMessage("peek()", (obj, _) => iteratorFunc(obj, i => i.Peek().Map(Some.Object) | (() => None.NoneValue)));
      registerMessage("reset()", (obj, _) => iteratorFunc(obj, i => i.Reset()));
      registerMessage("reverse()", (obj, _) => iteratorFunc(obj, i => i.Reverse()));
      registerMessage("join(_<String>)", (obj, message) => iteratorFunc<KString>(obj, message, (i, s) => i.Join(s.Value)));
      registerMessage("sort(_<Lambda>,asc:_<Boolean>)",
         (obj, message) => iteratorFunc<Lambda, KBoolean>(obj, message, (i, l, b) => i.Sort(l, b.Value)));
      registerMessage("sort(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Sort(l, true)));
      registerMessage("sort(_<Boolean>)", (obj, message) => iteratorFunc<KBoolean>(obj, message, (i, b) => i.Sort(b.Value)));
      registerMessage("sort()", (obj, _) => iteratorFunc(obj, i => i.Sort(true)));
      registerMessage("foldl".Selector("_", "_<Lambda>"),
         (obj, message) => iteratorFunc<IObject, Lambda>(obj, message, (i, o, l) => i.FoldLeft(o, l)));
      registerMessage("foldl(_)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.FoldLeft(l)));
      registerMessage("foldr".Selector("_", "_<Lambda>"),
         (obj, message) => iteratorFunc<IObject, Lambda>(obj, message, (i, o, l) => i.FoldRight(o, l)));
      registerMessage("foldr(_)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.FoldRight(l)));
      registerMessage("reducel".Selector("_", "_<Lambda>"),
         (obj, message) => iteratorFunc<IObject, Lambda>(obj, message, (i, o, l) => i.ReduceLeft(o, l)));
      registerMessage("reducel(_)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.ReduceLeft(l)));
      registerMessage("reducer".Selector("_", "_<Lambda>"),
         (obj, message) => iteratorFunc<IObject, Lambda>(obj, message, (i, o, l) => i.ReduceRight(o, l)));
      registerMessage("reducer(_)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.ReduceRight(l)));
      registerMessage("count".Selector("_<Lambda>"), (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Count(l)));
      registerMessage("count(of:_)", (obj, message) => iteratorFunc<IObject>(obj, message, (i, o) => i.Count(o)));
      registerMessage("map(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Map(l)));
      registerMessage("bind(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Map(l)));
      registerMessage("flatMap(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.FlatMap(l)));
      registerMessage("if(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.If(l)));
      registerMessage("ifNot(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.IfNot(l)));
      registerMessage("skip(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, j) => i.Skip(j.Value)));
      registerMessage("-(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, j) => i.Skip(j.Value)));
      registerMessage("skipWhile(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.SkipWhile(l)));
      registerMessage("skipUntil(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.SkipUntil(l)));
      registerMessage("take(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, j) => i.Take(j.Value)));
      registerMessage("+(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, j) => i.Take(j.Value)));
      registerMessage("takeWhile(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.TakeWhile(l)));
      registerMessage("takeUntil(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.TakeUntil(l)));
      registerMessage("index(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Index(l)));
      registerMessage("indexes(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Indexes(l)));
      registerMessage("zip(_<Collection>,_<Lambda>)",
         (obj, message) => iteratorFunc<IObject, Lambda>(obj, message, (i, c, l) => i.Zip((ICollection)c, l)));
      registerMessage("zip(_)", (obj, message) => iteratorFunc<IObject>(obj, message, (i, c) => i.Zip((ICollection)c)));
      registerMessage("min".get(), (obj, _) => iteratorFunc(obj, i => i.Min()));
      registerMessage("min(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Min(l)));
      registerMessage("max".get(), (obj, _) => iteratorFunc(obj, i => i.Max()));
      registerMessage("max(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Max(l)));
      registerMessage("first()", (obj, _) => iteratorFunc(obj, i => i.First()));
      registerMessage("first".Selector("_<Lambda>"), (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.First(l)));
      registerMessage("last()", (obj, _) => iteratorFunc(obj, i => i.Last()));
      registerMessage("last".Selector("_<Lambda>"), (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Last(l)));
      registerMessage("split(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Split(l)));
      registerMessage("split(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, j) => i.Split(j.Value)));
      registerMessage("random()", (obj, _) => iteratorFunc(obj, i => i.Random()));
      registerMessage("groupBy(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.GroupBy(l)));
      registerMessage("one(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.One(l)));
      registerMessage("none(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.None(l)));
      registerMessage("any(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Any(l)));
      registerMessage("all(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.All(l)));
      registerMessage("sum()", (obj, _) => iteratorFunc(obj, i => (IObject)i.Sum()));
      registerMessage("average()", (obj, _) => iteratorFunc(obj, i => (IObject)i.Average()));
      registerMessage("product()", (obj, _) => iteratorFunc(obj, i => (IObject)i.Product()));
      registerMessage("cross(_)", (obj, message) => iteratorFunc<IObject>(obj, message, (i, c) => i.Cross((ICollection)c)));
      registerMessage("cross(_,_)", (obj, message) => iteratorFunc<IObject, Lambda>(obj, message, (i, c, l) => i.Cross((ICollection)c, l)));
      registerMessage("by(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, j) => i.By(j.Value)));
      registerMessage("/(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, j) => i.By(j.Value)));
      registerMessage("window(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, j) => i.Window(j.Value)));
      registerMessage("//(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, j) => i.Window(j.Value)));
      registerMessage("distinct()", (obj, _) => iteratorFunc(obj, i => i.Distinct()));
      registerMessage("span".Selector("_<Lambda>"), (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Span(l)));
      registerMessage("span".Selector("_<Int>"), (obj, message) => iteratorFunc<Int>(obj, message, (i, j) => i.Span(j.Value)));
      registerMessage("shuffle()", (obj, _) => iteratorFunc(obj, i => i.Shuffle()));
      registerMessage("array()", (obj, _) => iteratorFunc(obj, i => i.ToArray()));
      registerMessage("list()", (obj, _) => iteratorFunc(obj, i => i.ToList()));
      registerMessage("tuple()", (obj, _) => iteratorFunc(obj, i => i.ToTuple()));
      registerMessage("dictionary".Selector("key:_<Lambda>", "value:_<Lambda>"),
         (obj, message) => iteratorFunc<Lambda, Lambda>(obj, message, (i, l1, l2) => i.ToDictionary(l1, l2)));
      registerMessage("dictionary()", (obj, _) => iteratorFunc(obj, i => i.ToDictionary()));
      registerMessage("each(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Each(l)));
      registerMessage("rotate(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, c) => i.Rotate(c.Value)));
      registerMessage("permutation(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, c) => i.Permutation(c.Value)));
      registerMessage("combination(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, c) => i.Combination(c.Value)));
      registerMessage("flatten()", (obj, _) => iteratorFunc(obj, i => i.Flatten()));
      registerMessage("copy()", (obj, _) => iteratorFunc(obj, i => i.Copy()));
      registerMessage("collect()", (obj, _) => iteratorFunc(obj, i => i.Collect()));
      registerMessage("*(_)", (obj, message) => iteratorFunc<IObject>(obj, message, (i1, i2) => i1.Apply((ICollection)i2)));
      registerMessage("format(_)", (obj, message) => iteratorFunc<KIndex>(obj, message, (i, index) => index.IndexOf(i.Collection)));
      registerMessage("replace(_<Lambda>,_<Lambda>)",
         (obj, message) => iteratorFunc<Lambda, Lambda>(obj, message, (i, l1, l2) => i.Replace(l1, l2)));
      registerMessage("set()", (obj, _) => iteratorFunc(obj, i => i.ToSet()));
      registerMessage("shape(_<Int>,_<Int>)",
         (obj, message) => iteratorFunc<Int, Int>(obj, message, (i, j, k) => i.Shape(j.Value, k.Value)));
      registerMessage("column(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, c) => i.Column(c.Value)));
      registerMessage("partition(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Partition(l)));
   }

   public virtual bool MatchCompatible(BaseClass otherClass) => Name == otherClass.Name;

   public virtual bool AssignCompatible(BaseClass otherClass) => MatchCompatible(otherClass);

   protected void rangeMessages()
   {
      registerMessage("succ".get(), (obj, _) => function<IObject>(obj, o => (IObject)((IRangeItem)o).Successor));
      registerMessage("pred".get(), (obj, _) => function<IObject>(obj, o => (IObject)((IRangeItem)o).Predecessor));
      registerMessage("range()", (obj, _) => function<IObject>(obj, o => ((IRangeItem)o).Range()));
   }

   public static IObject Invoke(IObject obj, Arguments arguments, Lambda lambda)
   {
      Fields fields;
      if (obj is UserObject uo)
      {
         fields = uo.Fields;
      }
      else
      {
         fields = new Fields();
         fields.New("self");
         fields.Assign("self", obj);
      }

      var _value = Machine.Current.Value.Invoke(lambda.Invokable, arguments, fields);
      if (_value is (true, var value))
      {
         return value;
      }
      else if (_value.Exception is (true, var exception))
      {
         throw exception;
      }
      else
      {
         return KVoid.Value;
      }
   }

   public static IObject Invoke(UserClass userClass, Arguments arguments, Lambda lambda)
   {
      return Machine.Current.Value.Invoke(lambda.Invokable, arguments, userClass.ClassFields)
         .RequiredCast<IObject>(() => "Return value required");
   }

   protected void messageNumberMessages()
   {
      registerMessage("-(()", (obj, _) => msgNumberFunction(obj, mn => mn.Negate()));
      registerMessage("sign()", (obj, _) => msgNumberFunction(obj, mn => mn.Sign()));
      registerMessage("^(_)", (obj, message) => msgNumberFunction(obj, message, (x, y) => x.Raise(y)));
      registerMessage("%(_)", (obj, message) => msgNumberFunction(obj, message, (x, y) => x.Remainder(y)));
      registerMessage("/(_)", (obj, message) => msgNumberFunction(obj, message, (x, y) => x.Divide(y)));
      registerMessage("/%(_)", (obj, message) => msgNumberFunction(obj, message, (x, y) => x.DivRem(y)));
      registerMessage("+(_)", (obj, message) => msgNumberFunction(obj, message, (x, y) => x.Add(y)));
      registerMessage("-(_)", (obj, message) => msgNumberFunction(obj, message, (x, y) => x.Subtract(y)));
      registerMessage("*(_)", (obj, message) => msgNumberFunction(obj, message, (x, y) => x.Multiply(y)));
      registerMessage("sin()", (obj, _) => msgNumberFunction(obj, mn => mn.Sin()));
      registerMessage("cos()", (obj, _) => msgNumberFunction(obj, mn => mn.Cos()));
      registerMessage("tan()", (obj, _) => msgNumberFunction(obj, mn => mn.Tan()));
      registerMessage("asin()", (obj, _) => msgNumberFunction(obj, mn => mn.Asin()));
      registerMessage("acos()", (obj, _) => msgNumberFunction(obj, mn => mn.Acos()));
      registerMessage("atan()", (obj, _) => msgNumberFunction(obj, mn => mn.Atan()));
      registerMessage("sinh()", (obj, _) => msgNumberFunction(obj, mn => mn.Sinh()));
      registerMessage("cosh()", (obj, _) => msgNumberFunction(obj, mn => mn.Cosh()));
      registerMessage("tanh()", (obj, _) => msgNumberFunction(obj, mn => mn.Tanh()));
      registerMessage("asinh()", (obj, _) => msgNumberFunction(obj, mn => mn.Asinh()));
      registerMessage("acosh()", (obj, _) => msgNumberFunction(obj, mn => mn.Acosh()));
      registerMessage("atanh()", (obj, _) => msgNumberFunction(obj, mn => mn.Atanh()));
      registerMessage("atan2(_)", (obj, message) => msgNumberFunction(obj, message, (x, y) => x.Atan2(y)));
      registerMessage("sqrt()", (obj, _) => msgNumberFunction(obj, mn => mn.Sqrt()));
      registerMessage("log()", (obj, _) => msgNumberFunction(obj, mn => mn.Log()));
      registerMessage("ln()", (obj, _) => msgNumberFunction(obj, mn => mn.Ln()));
      registerMessage("exp()", (obj, _) => msgNumberFunction(obj, mn => mn.Exp()));
      registerMessage("abs()", (obj, _) => msgNumberFunction(obj, mn => mn.Abs()));
      registerMessage("ceil()", (obj, _) => msgNumberFunction(obj, mn => mn.Ceiling()));
      registerMessage("floor()", (obj, _) => msgNumberFunction(obj, mn => mn.Floor()));
      registerMessage("frac()", (obj, _) => msgNumberFunction(obj, mn => mn.Fraction()));
      registerMessage("round(_,_<Int>)", (obj, message) => msgNumberFunction(obj, message, (x, y) => x.Round(y)));
   }

   protected void sliceableMessages()
   {
      registerMessage("$",
         (obj, message) => function<IObject, IObject>(obj, message, (o1, o2) =>
         {
            var sliceable = (ISliceable)o1;
            switch (o2)
            {
               case KRange range:
                  if (range.StopObj is End)
                  {
                     var length = sliceable.Length;
                     var start = range.StartObj;
                     if (start is Int { Value: < 0 } i)
                     {
                        start = (Int)wrapIndex(i.Value, length);
                     }

                     var newRange = new KRange((IRangeItem)start, (Int)(length - 1), range.Inclusive, range.Increment);
                     return sliceable.Slice(newRange);
                  }
                  else
                  {
                     return sliceable.Slice(range);
                  }

               case ICollection collection:
                  return sliceable.Slice(collection);
               default:
                  var tuple = new KTuple(o2);
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
      registerMessage("find(_<TextFinding>)", (obj, message) => apply(obj, message, (s, tf) => s.Find(tf, 0, false)));
      registerMessage("find(_<TextFinding>,startAt:_<Int>)",
         (obj, message) => apply1<Int>(obj, message, (s, tf, i) => s.Find(tf, i.Value, false)));
      registerMessage("find(_<TextFinding>,reverse:_<Boolean>)",
         (obj, message) => apply1<KBoolean>(obj, message, (s, tf, b) => s.Find(tf, 0, b.Value)));
      registerMessage("find(_<TextFinding>,startAt:_<Int>,reverse:_<Boolean>)",
         (obj, message) => apply2<Int, KBoolean>(obj, message, (s, tf, i, b) => s.Find(tf, i.Value, b.Value)));
      registerMessage("find(all:_<TextFinding>)", (obj, message) => apply(obj, message, (s, tf) => s.FindAll(tf)));
      registerMessage("replace(_<TextFinding>,new:_<String>)",
         (obj, message) => apply1<KString>(obj, message, (s1, tf, s2) => s1.Replace(tf, s2.Value, false)));
      registerMessage("replace(_<TextFinding>,new:_<String>,reverse:_<Boolean>)",
         (obj, message) => apply2<KString, KBoolean>(obj, message, (s1, tf, s2, b) => s1.Replace(tf, s2.Value, b.Value)));
      registerMessage("replace(all:_<TextFinding>,new:_<String>)",
         (obj, message) => apply1<KString>(obj, message, (s1, tf, s2) => s1.ReplaceAll(tf, s2.Value)));
      registerMessage("replace(_<TextFinding>,with:_<Lambda>)",
         (obj, message) => apply1<Lambda>(obj, message, (s, tf, l) => s.Replace(tf, l, false)));
      registerMessage("replace(_<TextFinding>,with:_<Lambda>,reverse:_<Boolean>)",
         (obj, message) => apply2<Lambda, KBoolean>(obj, message, (s, tf, l, b) => s.Replace(tf, l, b.Value)));
      registerMessage("replace(all:_<TextFinding>,with:_<Lambda>)",
         (obj, message) => apply1<Lambda>(obj, message, (s, tf, l) => s.ReplaceAll(tf, l)));
      registerMessage("split(on:_<TextFinding>)", (obj, message) => apply(obj, message, (s, tf) => s.Split(tf)));
      registerMessage("partition(_<TextFinding>)", (obj, message) => apply(obj, message, (s, tf) => s.Partition(tf, false)));
      registerMessage("partition(_<TextFinding>,reverse:_<Boolean>)",
         (obj, message) => apply1<KBoolean>(obj, message, (s, tf, b) => s.Partition(tf, b.Value)));
      registerMessage("count(_<String>)", (obj, message) => apply(obj, message, (s, tf) => s.Count(tf)));
      registerMessage("count(_<String>,_<Lambda>)", (obj, message) => apply1<Lambda>(obj, message, (s, tf, l) => s.Count(tf, l)));

      return;

      IObject apply2<T1, T2>(IObject obj, Message message, Func<KString, ITextFinding, T1, T2, IObject> func)
         where T1 : IObject
         where T2 : IObject
      {
         KString input = obj.AsString;
         var textFinding = (ITextFinding)message.Arguments[0];
         var arg1 = (T1)message.Arguments[1];
         var arg2 = (T2)message.Arguments[2];

         return func(input, textFinding, arg1, arg2);
      }

      IObject apply1<T>(IObject obj, Message message, Func<KString, ITextFinding, T, IObject> func)
         where T : IObject
      {
         KString input = obj.AsString;
         var textFinding = (ITextFinding)message.Arguments[0];
         var arg1 = (T)message.Arguments[1];

         return func(input, textFinding, arg1);
      }

      IObject apply(IObject obj, Message message, Func<KString, ITextFinding, IObject> func)
      {
         KString input = obj.AsString;
         var textFinding = (ITextFinding)message.Arguments[0];

         return func(input, textFinding);
      }
   }

   protected void monadMessages()
   {
      registerMessage("bind(_<Lambda>)", (obj, message) => ((IMonad)obj).Bind((Lambda)message.Arguments[0]));
      registerMessage("unit(_)", (obj, message) => ((IMonad)obj).Unit(message.Arguments[0]));
   }

   protected void findAndIndexMessages()
   {
      messages["index(of:_)"] = (obj, msg) => ((IFindIndex)obj).IndexOf(msg.Arguments[0]);
      messages["reverseIndex(of:_)"] = (obj, msg) => ((IFindIndex)obj).ReverseIndexOf(msg.Arguments[0]);
      messages["find(all:_<Lambda>)"] = (obj, msg) => ((IFindIndex)obj).FindAll((Lambda)msg.Arguments[0]);
      messages["first(_<Lambda>)"] = (obj, msg) => ((IFindIndex)obj).First((Lambda)msg.Arguments[0]);
      messages["last(_<Lambda>)"] = (obj, msg) => ((IFindIndex)obj).Last((Lambda)msg.Arguments[0]);
   }

   public void RegisterInclusion(Inclusion inclusion) => inclusions[inclusion.Name] = inclusion;

   public bool Includes(string inclusionName) => inclusions.ContainsKey(inclusionName);
}