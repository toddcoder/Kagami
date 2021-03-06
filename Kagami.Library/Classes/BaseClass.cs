﻿using System;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Objects;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Classes.ClassFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Operations.NumericFunctions;
using static Core.Arrays.ArrayFunctions;
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
         dynamicInvoke = (obj, _) => throw messageNotFound(classOf(obj), _.Selector);
         classDynamicInvoke = _ => throw messageNotFound(this, _.Selector);
         classFields = new Fields();
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
         registerMessage("string".get(), (obj, _) => String.StringObject(obj.AsString));
         registerMessage("image".get(), (obj, _) => String.StringObject(obj.Image));
         registerMessage("hash".get(), (obj, _) => Int.IntObject(obj.Hash));
         registerMessage("equals(_)", (obj, _) => Boolean.BooleanObject(obj.IsEqualTo(_.Arguments[0])));
         registerMessage("className".get(), (obj, _) => String.StringObject(obj.ClassName));
         registerMessage("class".get(), (obj, _) => new Class(obj.ClassName));
         registerMessage("match(_)", match);
         messages["isNumber".get()] = (_, _) => Boolean.False;
         registerMessage("send(_,_)",
            (obj, _) => function<IObject, String>(obj, _, (o, n) => sendMessage(o, n.Value, _.Arguments.Pass(1))));
         registerMessage("respondsTo(_)", (obj, _) => (Boolean)classOf(obj).RespondsTo(_.Arguments[0].AsString));
         registerMessage("seq(_)", (obj, _) => new OpenRange(obj, (Lambda)_.Arguments[0]));
         registerMessage("format(_)", (obj, _) => format(obj, _.Arguments[0].AsString));
      }

      protected static String format(IObject obj, string formattingString)
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
         registerClassMessage("name".get(), (_, _) => String.StringObject(Name));
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
            return messages.ContainsKey(selector) ? messages[selector](obj, message) : DynamicInvoke(obj, message);
         }
         else
         {
            throw messageNotFound(classOf(obj), selector);
         }
      }

      protected IObject invokeDirectly(IObject obj, Message message)
      {
         var result = messages[message.Selector];
         if (result == null)
         {
            throw messageNotFound(classOf(obj), message.Selector);
         }
         else
         {
            return result(obj, message);
         }
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
         registerMessage("+",
            (obj, _) => function(obj, _, (x, y) => x + y, (x, y) => x + y, (x, y) => x + y, (x, y) => x.Add(y), "+"));
         registerMessage("-", (obj, _) => function(obj, _, (x, y) => x - y, (x, y) => x - y, (x, y) => x - y,
            (x, y) => x.Subtract(y), "-"));
         registerMessage("*", (obj, _) => function(obj, _, (x, y) => x * y, (x, y) => x * y, (x, y) => x * y,
            (x, y) => x.Multiply(y), "*"));
         registerMessage("/", (obj, _) => function(obj, _, (x, y) => x / y, (x, y) => x.Divide(y), "/"));
         registerMessage("//", integerDivision);
         registerMessage("%", (obj, _) => function(obj, _, (x, y) => x % y, (x, y) => x % y, (x, y) => x % y,
            (x, y) => x.Remainder(y), "%"));
         registerMessage("^", (obj, _) => function(obj, _, (x, y) => Math.Pow(x, y), (x, y) => x.Raise(y), "^"));
         registerMessage("atan2", (obj, _) => function(obj, _, (x, y) => Math.Atan2(y, x), (x, y) => x.Atan2(y), "atan2"));
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
         messages["isNumber".get()] = (_, _) => Boolean.True;
         registerMessage("isZero".get(), (obj, _) => function(obj, numeric => (Boolean)numeric.IsZero));
         registerMessage("isPositive".get(), (obj, _) => function(obj, numeric => (Boolean)numeric.IsPositive));
         registerMessage("isNegative".get(), (obj, _) => function(obj, numeric => (Boolean)numeric.IsNegative));
         registerMessage("isPrimitive".get(), (obj, _) => function(obj, numeric => (Boolean)numeric.IsPrimitive));
         registerMessage("zfill(_<Int>)",
            (obj, _) => function<IObject, Int>(obj, _, (numeric, i) => ((INumeric)numeric).ZFill(i.Value)));
      }

      protected void numericConversionMessages()
      {
         registerMessage("i".get(),
            (obj, _) => function(obj, i => i, d => (int)d, b => b, m => (Int)((INumeric)m).ToInt(), "i".get()));
         registerMessage("f".get(),
            (obj, _) => function(obj, i => i, d => d, b => b, m => (Float)((INumeric)m).ToFloat(), "f".get()));
         registerMessage("b".get(),
            (obj, _) => function(obj, i => (byte)i, d => (byte)d, b => b, m => (Byte)((INumeric)m).ToByte(), "b".get()));
      }

      protected void collectionMessages()
      {
         registerMessage("getIterator(_<Boolean>)",
            (obj, _) => collectionFunc<Boolean>(obj, _, (c, l) => (IObject)c.GetIterator(l.Value)));
         registerMessage("length".get(), (obj, _) => collectionFunc(obj, c => c.Length));
         registerMessage("in(_)", (obj, _) => collectionFunc<IObject>(obj, _, (c, i) => c.In(i)));
         registerMessage("notIn(_)", (obj, _) => collectionFunc<IObject>(obj, _, (c, i) => c.NotIn(i)));
         registerMessage("*(_<Int>)", (obj, _) => collectionFunc<Int>(obj, _, (c, i) => c.Times(i.Value)));
         registerMessage("*(_<String>)",
            (obj, _) => collectionFunc<String>(obj, _, (c, connector) => c.MakeString(connector.Value)));
         registerMessage("indexed()", (obj, _) => collectionFunc(obj, c => (IObject)c.GetIndexedIterator()));
         registerMessage("[](_<SkipTake>)", (obj, _) => ((ISkipTakeable)obj)[(SkipTake)_.Arguments[0]]);

         loadIteratorMessages();
      }

      protected void mutableCollectionMessages()
      {
         registerMessage("<<(_)", (obj, _) => function<IObject, IObject>(obj, _, (o, v) => ((IMutableCollection)o).Append(v)));
         registerMessage("append(_)", (obj, _) => function<IObject, IObject>(obj, _, (o, v) => ((IMutableCollection)o).Append(v)));
         registerMessage(">>(_)", (obj, _) => function<IObject, IObject>(obj, _, (o, v) => ((IMutableCollection)o).Remove(v)));
         registerMessage("remove(_)", (obj, _) => function<IObject, IObject>(obj, _, (o, v) => ((IMutableCollection)o).Remove(v)));
         registerMessage("-(_)", (obj, _) => function<IObject, IObject>(obj, _, (o, v) => ((IMutableCollection)o).Remove(v)));
         registerMessage("remove(at:_<Int>)",
            (obj, _) => function<IObject, Int>(obj, _, (o, i) => ((IMutableCollection)o).RemoveAt(i.Value)));
         registerMessage("remove(all:_)",
            (obj, _) => function<IObject, IObject>(obj, _, (o, v) => ((IMutableCollection)o).RemoveAll(v)));
         registerMessage("insert(at:_<Int>,value:_)",
            (obj, _) => function<IObject, Int, IObject>(obj, _, (o, i, v) => ((IMutableCollection)o).InsertAt(i.Value, v)));
         registerMessage("isEmpty".get(), (obj, _) => function<IObject>(obj, o => ((IMutableCollection)o).IsEmpty));
         registerMessage("assign(_,_)",
            (obj, _) => function<IObject, IObject, IObject>(obj, _, (o, i, v) => ((IMutableCollection)o).Assign(i, v)));
      }

      protected void loadIteratorMessages()
      {
         alternateMessages.AddRange(array<string>("collection".get(), "isLazy".get(), "next()", "peek()", "reset()", "reverse()",
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
            "column(_<Int>)", "step(_<Int>)"));

         dynamicInvoke = (obj, _) =>
         {
            var iterator = (IObject)((ICollection)obj).GetIterator(false);
            return classOf(iterator).SendMessage(iterator, _);
         };
      }

      protected void iteratorMessages()
      {
         registerMessage("collection".get(), (obj, _) => iteratorFunc(obj, i => (IObject)i.Collection));
         registerMessage("isLazy".get(), (obj, _) => iteratorFunc(obj, i => (Boolean)i.IsLazy));
         registerMessage("next()", (obj, _) => iteratorFunc(obj, i => i.Next().Map(Some.Object).DefaultTo(() => None.NoneValue)));
         registerMessage("peek()", (obj, _) => iteratorFunc(obj, i => i.Peek().Map(Some.Object).DefaultTo(() => None.NoneValue)));
         registerMessage("reset()", (obj, _) => iteratorFunc(obj, i => i.Reset()));
         registerMessage("reverse()", (obj, _) => iteratorFunc(obj, i => i.Reverse()));
         registerMessage("join(_<String>)", (obj, _) => iteratorFunc<String>(obj, _, (i, s) => i.Join(s.Value)));
         registerMessage("sort(_<Lambda>,asc:_<Boolean>)",
            (obj, _) => iteratorFunc<Lambda, Boolean>(obj, _, (i, l, b) => i.Sort(l, b.Value)));
         registerMessage("sort(_<Lambda>)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.Sort(l, true)));
         registerMessage("sort(_<Boolean>)", (obj, _) => iteratorFunc<Boolean>(obj, _, (i, b) => i.Sort(b.Value)));
         registerMessage("sort()", (obj, _) => iteratorFunc(obj, i => i.Sort(true)));
         registerMessage("foldl".Selector("_", "_<Lambda>"),
            (obj, _) => iteratorFunc<IObject, Lambda>(obj, _, (i, o, l) => i.FoldLeft(o, l)));
         registerMessage("foldl(_)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.FoldLeft(l)));
         registerMessage("foldr".Selector("_", "_<Lambda>"),
            (obj, _) => iteratorFunc<IObject, Lambda>(obj, _, (i, o, l) => i.FoldRight(o, l)));
         registerMessage("foldr(_)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.FoldRight(l)));
         registerMessage("reducel".Selector("_", "_<Lambda>"),
            (obj, _) => iteratorFunc<IObject, Lambda>(obj, _, (i, o, l) => i.ReduceLeft(o, l)));
         registerMessage("reducel(_)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.ReduceLeft(l)));
         registerMessage("reducer".Selector("_", "_<Lambda>"),
            (obj, _) => iteratorFunc<IObject, Lambda>(obj, _, (i, o, l) => i.ReduceRight(o, l)));
         registerMessage("reducer(_)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.ReduceRight(l)));
         registerMessage("count".Selector("_<Lambda>"), (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.Count(l)));
         registerMessage("count(of:_)", (obj, _) => iteratorFunc<IObject>(obj, _, (i, o) => i.Count(o)));
         registerMessage("map(_<Lambda>)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.Map(l)));
         registerMessage("bind(_<Lambda>)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.Map(l)));
         registerMessage("flatMap(_<Lambda>)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.FlatMap(l)));
         registerMessage("if(_<Lambda>)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.If(l)));
         registerMessage("ifNot(_<Lambda>)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.IfNot(l)));
         registerMessage("skip(_<Int>)", (obj, _) => iteratorFunc<Int>(obj, _, (i, j) => i.Skip(j.Value)));
         registerMessage("-(_<Int>)", (obj, _) => iteratorFunc<Int>(obj, _, (i, j) => i.Skip(j.Value)));
         registerMessage("skipWhile(_<Lambda>)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.SkipWhile(l)));
         registerMessage("skipUntil(_<Lambda>)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.SkipUntil(l)));
         registerMessage("take(_<Int>)", (obj, _) => iteratorFunc<Int>(obj, _, (i, j) => i.Take(j.Value)));
         registerMessage("+(_<Int>)", (obj, _) => iteratorFunc<Int>(obj, _, (i, j) => i.Take(j.Value)));
         registerMessage("takeWhile(_<Lambda>)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.TakeWhile(l)));
         registerMessage("takeUntil(_<Lambda>)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.TakeUntil(l)));
         registerMessage("index(_<Lambda>)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.Index(l)));
         registerMessage("indexes(_<Lambda>)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.Indexes(l)));
         registerMessage("zip(_<Collection>,_<Lambda>)",
            (obj, _) => iteratorFunc<IObject, Lambda>(obj, _, (i, c, l) => i.Zip((ICollection)c, l)));
         registerMessage("zip(_)", (obj, _) => iteratorFunc<IObject>(obj, _, (i, c) => i.Zip((ICollection)c)));
         registerMessage("min".get(), (obj, _) => iteratorFunc(obj, i => i.Min()));
         registerMessage("min(_<Lambda>)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.Min(l)));
         registerMessage("max".get(), (obj, _) => iteratorFunc(obj, i => i.Max()));
         registerMessage("max(_<Lambda>)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.Max(l)));
         registerMessage("first()", (obj, _) => iteratorFunc(obj, i => i.First()));
         registerMessage("first".Selector("_<Lambda>"), (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.First(l)));
         registerMessage("last()", (obj, _) => iteratorFunc(obj, i => i.Last()));
         registerMessage("last".Selector("_<Lambda>"), (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.Last(l)));
         registerMessage("split(_<Lambda>)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.Split(l)));
         registerMessage("split(_<Int>)", (obj, _) => iteratorFunc<Int>(obj, _, (i, j) => i.Split(j.Value)));
         registerMessage("random()", (obj, _) => iteratorFunc(obj, i => i.Random()));
         registerMessage("groupBy(_<Lambda>)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.GroupBy(l)));
         registerMessage("one(_<Lambda>)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.One(l)));
         registerMessage("none(_<Lambda>)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.None(l)));
         registerMessage("any(_<Lambda>)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.Any(l)));
         registerMessage("all(_<Lambda>)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.All(l)));
         registerMessage("sum()", (obj, _) => iteratorFunc(obj, i => (IObject)i.Sum()));
         registerMessage("average()", (obj, _) => iteratorFunc(obj, i => (IObject)i.Average()));
         registerMessage("product()", (obj, _) => iteratorFunc(obj, i => (IObject)i.Product()));
         registerMessage("cross(_)", (obj, _) => iteratorFunc<IObject>(obj, _, (i, c) => i.Cross((ICollection)c)));
         registerMessage("cross(_,_)", (obj, _) => iteratorFunc<IObject, Lambda>(obj, _, (i, c, l) => i.Cross((ICollection)c, l)));
         registerMessage("by(_<Int>)", (obj, _) => iteratorFunc<Int>(obj, _, (i, j) => i.By(j.Value)));
         registerMessage("/(_<Int>)", (obj, _) => iteratorFunc<Int>(obj, _, (i, j) => i.By(j.Value)));
         registerMessage("window(_<Int>)", (obj, _) => iteratorFunc<Int>(obj, _, (i, j) => i.Window(j.Value)));
         registerMessage("//(_<Int>)", (obj, _) => iteratorFunc<Int>(obj, _, (i, j) => i.Window(j.Value)));
         registerMessage("distinct()", (obj, _) => iteratorFunc(obj, i => i.Distinct()));
         registerMessage("span".Selector("_<Lambda>"), (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.Span(l)));
         registerMessage("span".Selector("_<Int>"), (obj, _) => iteratorFunc<Int>(obj, _, (i, j) => i.Span(j.Value)));
         registerMessage("shuffle()", (obj, _) => iteratorFunc(obj, i => i.Shuffle()));
         registerMessage("array()", (obj, _) => iteratorFunc(obj, i => i.ToArray()));
         registerMessage("list()", (obj, _) => iteratorFunc(obj, i => i.ToList()));
         registerMessage("tuple()", (obj, _) => iteratorFunc(obj, i => i.ToTuple()));
         registerMessage("dictionary".Selector("key:_<Lambda>", "value:_<Lambda>"),
            (obj, _) => iteratorFunc<Lambda, Lambda>(obj, _, (i, l1, l2) => i.ToDictionary(l1, l2)));
         registerMessage("dictionary()", (obj, _) => iteratorFunc(obj, i => i.ToDictionary()));
         registerMessage("each(_<Lambda>)", (obj, _) => iteratorFunc<Lambda>(obj, _, (i, l) => i.Each(l)));
         registerMessage("rotate(_<Int>)", (obj, _) => iteratorFunc<Int>(obj, _, (i, c) => i.Rotate(c.Value)));
         registerMessage("permutation(_<Int>)", (obj, _) => iteratorFunc<Int>(obj, _, (i, c) => i.Permutation(c.Value)));
         registerMessage("combination(_<Int>)", (obj, _) => iteratorFunc<Int>(obj, _, (i, c) => i.Combination(c.Value)));
         registerMessage("flatten()", (obj, _) => iteratorFunc(obj, i => i.Flatten()));
         registerMessage("copy()", (obj, _) => iteratorFunc(obj, i => i.Copy()));
         registerMessage("collect()", (obj, _) => iteratorFunc(obj, i => i.Collect()));
         registerMessage("*(_)", (obj, _) => iteratorFunc<IObject>(obj, _, (i1, i2) => i1.Apply((ICollection)i2)));
         registerMessage("format(_)", (obj, _) => iteratorFunc<Index>(obj, _, (i, index) => index.IndexOf(i.Collection)));
         registerMessage("replace(_<Lambda>,_<Lambda>)",
            (obj, _) => iteratorFunc<Lambda, Lambda>(obj, _, (i, l1, l2) => i.Replace(l1, l2)));
         registerMessage("set()", (obj, _) => iteratorFunc(obj, i => i.ToSet()));
         registerMessage("shape(_<Int>,_<Int>)",
            (obj, _) => iteratorFunc<Int, Int>(obj, _, (i, j, k) => i.Shape(j.Value, k.Value)));
         registerMessage("column(_<Int>)", (obj, _) => iteratorFunc<Int>(obj, _, (i, c) => i.Column(c.Value)));
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

         if (Machine.Current.Invoke(lambda.Invokable, arguments, fields).If(out var value, out var anyException))
         {
            return value;
         }
         else if (anyException.If(out var exception))
         {
            throw exception;
         }
         else
         {
            return Void.Value;
         }
      }

      public static IObject Invoke(UserClass userClass, Arguments arguments, Lambda lambda)
      {
         return Machine.Current.Invoke(lambda.Invokable, arguments, userClass.ClassFields)
            .RequiredCast<IObject>(() => "Return value required");
      }

      protected void messageNumberMessages()
      {
         registerMessage("-(", (obj, _) => msgNumberFunction(obj, mn => mn.Negate()));
         registerMessage("sign", (obj, _) => msgNumberFunction(obj, mn => mn.Sign()));
         registerMessage("^", (obj, _) => msgNumberFunction(obj, _, (x, y) => x.Raise(y)));
         registerMessage("mod", (obj, _) => msgNumberFunction(obj, _, (x, y) => x.Remainder(y)));
         registerMessage("/", (obj, _) => msgNumberFunction(obj, _, (x, y) => x.Divide(y)));
         registerMessage("/%", (obj, _) => msgNumberFunction(obj, _, (x, y) => x.DivRem(y)));
         registerMessage("+", (obj, _) => msgNumberFunction(obj, _, (x, y) => x.Add(y)));
         registerMessage("-", (obj, _) => msgNumberFunction(obj, _, (x, y) => x.Subtract(y)));
         registerMessage("*", (obj, _) => msgNumberFunction(obj, _, (x, y) => x.Multiply(y)));
         registerMessage("sin", (obj, _) => msgNumberFunction(obj, mn => mn.Sin()));
         registerMessage("cos", (obj, _) => msgNumberFunction(obj, mn => mn.Cos()));
         registerMessage("tan", (obj, _) => msgNumberFunction(obj, mn => mn.Tan()));
         registerMessage("asin", (obj, _) => msgNumberFunction(obj, mn => mn.Asin()));
         registerMessage("acos", (obj, _) => msgNumberFunction(obj, mn => mn.Acos()));
         registerMessage("atan", (obj, _) => msgNumberFunction(obj, mn => mn.Atan()));
         registerMessage("sinh", (obj, _) => msgNumberFunction(obj, mn => mn.Sinh()));
         registerMessage("cosh", (obj, _) => msgNumberFunction(obj, mn => mn.Cosh()));
         registerMessage("tanh", (obj, _) => msgNumberFunction(obj, mn => mn.Tanh()));
         registerMessage("asinh", (obj, _) => msgNumberFunction(obj, mn => mn.Asinh()));
         registerMessage("acosh", (obj, _) => msgNumberFunction(obj, mn => mn.Acosh()));
         registerMessage("atanh", (obj, _) => msgNumberFunction(obj, mn => mn.Atanh()));
         registerMessage("atan2", (obj, _) => msgNumberFunction(obj, _, (x, y) => x.Atan2(y)));
         registerMessage("sqrt", (obj, _) => msgNumberFunction(obj, mn => mn.Sqrt()));
         registerMessage("log", (obj, _) => msgNumberFunction(obj, mn => mn.Log()));
         registerMessage("ln", (obj, _) => msgNumberFunction(obj, mn => mn.Ln()));
         registerMessage("exp", (obj, _) => msgNumberFunction(obj, mn => mn.Exp()));
         registerMessage("abs", (obj, _) => msgNumberFunction(obj, mn => mn.Abs()));
         registerMessage("ceil", (obj, _) => msgNumberFunction(obj, mn => mn.Ceiling()));
         registerMessage("floor", (obj, _) => msgNumberFunction(obj, mn => mn.Floor()));
         registerMessage("frac", (obj, _) => msgNumberFunction(obj, mn => mn.Fraction()));
         registerMessage("round", (obj, _) => msgNumberFunction(obj, _, (x, y) => x.Round(y)));
      }

      protected void sliceableMessages()
      {
         registerMessage("$",
            (obj, _) => function<IObject, IObject>(obj, _, (o1, o2) =>
            {
               var sliceable = (ISliceable)o1;
               switch (o2)
               {
                  case Range range:
                     if (range.StopObj is End)
                     {
                        var length = sliceable.Length;
                        var start = range.StartObj;
                        if (start is Int { Value: < 0 } i)
                        {
                           start = (Int)wrapIndex(i.Value, length);
                        }

                        var newRange = new Range((IRangeItem)start, (Int)(length - 1), range.Inclusive, range.Increment);
                        return sliceable.Slice(newRange);
                     }
                     else
                     {
                        return sliceable.Slice(range);
                     }

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
         IObject apply(IObject obj, Message _, Func<String, ITextFinding, IObject> func)
         {
            String input = obj.AsString;
            var textFinding = (ITextFinding)_.Arguments[0];

            return func(input, textFinding);
         }

         IObject apply1<T>(IObject obj, Message _, Func<String, ITextFinding, T, IObject> func)
            where T : IObject
         {
            String input = obj.AsString;
            var textFinding = (ITextFinding)_.Arguments[0];
            var arg1 = (T)_.Arguments[1];

            return func(input, textFinding, arg1);
         }

         IObject apply2<T1, T2>(IObject obj, Message _, Func<String, ITextFinding, T1, T2, IObject> func)
            where T1 : IObject
            where T2 : IObject
         {
            String input = obj.AsString;
            var textFinding = (ITextFinding)_.Arguments[0];
            var arg1 = (T1)_.Arguments[1];
            var arg2 = (T2)_.Arguments[2];

            return func(input, textFinding, arg1, arg2);
         }

         registerMessage("find(_<TextFinding>)", (obj, _) => apply(obj, _, (s, tf) => s.Find(tf, 0, false)));
         registerMessage("find(_<TextFinding>,startAt:_<Int>)",
            (obj, _) => apply1<Int>(obj, _, (s, tf, i) => s.Find(tf, i.Value, false)));
         registerMessage("find(_<TextFinding>,reverse:_<Boolean>)",
            (obj, _) => apply1<Boolean>(obj, _, (s, tf, b) => s.Find(tf, 0, b.Value)));
         registerMessage("find(_<TextFinding>,startAt:_<Int>,reverse:_<Boolean>)",
            (obj, _) => apply2<Int, Boolean>(obj, _, (s, tf, i, b) => s.Find(tf, i.Value, b.Value)));
         registerMessage("find(all:_<TextFinding>)", (obj, _) => apply(obj, _, (s, tf) => s.FindAll(tf)));
         registerMessage("replace(_<TextFinding>,new:_<String>)",
            (obj, _) => apply1<String>(obj, _, (s1, tf, s2) => s1.Replace(tf, s2.Value, false)));
         registerMessage("replace(_<TextFinding>,new:_<String>,reverse:_<Boolean>)",
            (obj, _) => apply2<String, Boolean>(obj, _, (s1, tf, s2, b) => s1.Replace(tf, s2.Value, b.Value)));
         registerMessage("replace(all:_<TextFinding>,new:_<String>)",
            (obj, _) => apply1<String>(obj, _, (s1, tf, s2) => s1.ReplaceAll(tf, s2.Value)));
         registerMessage("replace(_<TextFinding>,with:_<Lambda>)",
            (obj, _) => apply1<Lambda>(obj, _, (s, tf, l) => s.Replace(tf, l, false)));
         registerMessage("replace(_<TextFinding>,with:_<Lambda>,reverse:_<Boolean>)",
            (obj, _) => apply2<Lambda, Boolean>(obj, _, (s, tf, l, b) => s.Replace(tf, l, b.Value)));
         registerMessage("replace(all:_<TextFinding>,with:_<Lambda>)",
            (obj, _) => apply1<Lambda>(obj, _, (s, tf, l) => s.ReplaceAll(tf, l)));
         registerMessage("split(on:_<TextFinding>)", (obj, _) => apply(obj, _, (s, tf) => s.Split(tf)));
         registerMessage("partition(_<TextFinding>)", (obj, _) => apply(obj, _, (s, tf) => s.Partition(tf, false)));
         registerMessage("partition(_<TextFinding>,reverse:_<Boolean>)",
            (obj, _) => apply1<Boolean>(obj, _, (s, tf, b) => s.Partition(tf, b.Value)));
         registerMessage("count(_<String>)", (obj, _) => apply(obj, _, (s, tf) => s.Count(tf)));
         registerMessage("count(_<String>,_<Lambda>)", (obj, _) => apply1<Lambda>(obj, _, (s, tf, l) => s.Count(tf, l)));
      }

      protected void monadMessage()
      {
         registerMessage("bind(_<Lambda>)", (obj, _) => ((IMonad)obj).Bind((Lambda)_.Arguments[0]));
         registerMessage("unit(_)", (obj, _) => ((IMonad)obj).Unit(_.Arguments[0]));
      }
   }
}