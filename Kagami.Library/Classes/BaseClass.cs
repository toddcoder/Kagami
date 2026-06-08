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
using ObjectFunctions = Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Classes;

public abstract class BaseClass : IEquatable<BaseClass>
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

   public Guid Id { get; set; } = Guid.NewGuid();

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

   protected void registerIterMessage(Selector selector, Func<IObject, Message, IObject> function)
   {
      alternateMessages.Add(selector);
      registerMessage(selector, function);
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
      registerMessage("send(_<String>,_...)",
         (obj, message) => function<IObject, KString>(obj, message, (o, n) => sendMessage(o, n.Value, message.Arguments.Pass(1))));
      registerMessage("send(_<String>)",
         (obj, message) => function<IObject, KString>(obj, message, (o, n) => sendMessage(o, n.Value, Arguments.Empty)));
      registerMessage("receives(_)", (obj, message) => (KBoolean)classOf(obj).RespondsTo((Selector)message.Arguments[0]));
      registerMessage("seq(_)", (obj, message) => new OpenRange(obj, (Lambda)message.Arguments[0]));
      registerMessage("format(_<String>)", (obj, message) => format(obj, message.Arguments[0].AsString));
      registerMessage("format(_<Array>)", (obj, message) => formatArray(obj, message.Arguments[0]));
      registerMessage("format(_<Lambda>)", (obj, message) => formatLambda(obj, message));
      registerMessage("objId".get(), (obj, _) => KString.StringObject(obj.Id.ToString()));
      registerMessage("isTrue".get(), (obj, _) => KBoolean.BooleanObject(obj.IsTrue));
      registerMessage("numberize()", (_, _) => Undefined.Value);
   }

   protected static KString format(IObject obj, string formattingString)
   {
      if (obj is Date date)
      {
         return date.Format(formattingString);
      }
      else if (obj is IFormattable formattable)
      {
         return formattable.Format(formattingString);
      }
      else if (formattingString.Contains(' '))
      {
         return formatArray(obj, formattingString.Split(' '));
      }
      else if (formattingString == "i")
      {
         return obj.Image;
      }
      else
      {
         return obj.AsString;
      }
   }

   protected static string formatArray(IObject obj, string[] formats)
   {
      if (obj is IFormattable formattable)
      {
         return ObjectFunctions.format(formattable, formats).Value;
      }
      else
      {
         return obj.AsString;
      }
   }

   protected static IObject formatArray(IObject obj, IObject arrayAsObject)
   {
      if (arrayAsObject is KArray kArray)
      {
         switch (obj)
         {
            case LazyString lazyString:
            {
               return new Formatter(lazyString, kArray);
            }
            case Formatter formatter:
            {
               return formatter.Clone(kArray);
            }
            case IFormattable formattable:
            {
               string[] array = [.. kArray.List.Select(o => o.AsString)];
               return ObjectFunctions.format(formattable, array);
            }
            default:
               return KString.StringObject(obj.AsString);
         }
      }
      else
      {
         return KString.StringObject(obj.AsString);
      }
   }

   protected static KString formatLambda(IObject obj, Message message)
   {
      if (obj is IFormattable formattable && message.Arguments.Length > 0 && message.Arguments[0] is Lambda lambda)
      {
         return formattable.Format(lambda);
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
      registerClassMessage("equals(_)", (bc, msg) => classFunc<BaseClass, KBoolean>(bc, msg, (b1, b2) => (KBoolean)b1.Equals(b2)));
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
      if (message.Arguments.Length > 0 && message.Arguments[0] is Junction junction)
      {
         return invokeOnJunction(obj, junction, message);
      }

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
      }
      else
      {
         return DynamicInvoke(obj, message);
      }
   }

   protected IObject invokeOnJunction(IObject obj, Junction junction, Message message)
   {
      List<IObject> newItems = [];
      foreach (var junctionItem in junction.Items)
      {
         Selector newSelector = message.Selector.ToString().Replace("<Junction>", "");
         var newMessage = new Message(newSelector, [junctionItem, .. message.Arguments.Skip(1)]);
         var result = SendMessage(obj, newMessage);
         newItems.Add(result);
      }

      return junction.NewJunction(newItems).Flatten();
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
      registerMessage("trunc()",
         (obj, _) => function(obj, x => x, x => Math.Truncate(x), x => x, x => (Float)x.Trunc(), "trunc()"));
      registerMessage("floor()", (obj, _) => function(obj, x => x, x => Math.Floor(x), x => x, x => (Float)x.Floor(), "floor()"));
      registerMessage("frac()", (obj, _) => function(obj, _ => 0, x => x - (int)x, _ => 0, x => (Float)x.Fraction(), "frac()"));
      messages["isNumber".get()] = (_, _) => KBoolean.True;
      registerMessage("isZero".get(), (obj, _) => function(obj, numeric => (KBoolean)numeric.IsZero));
      registerMessage("isPositive".get(), (obj, _) => function(obj, numeric => (KBoolean)numeric.IsPositive));
      registerMessage("isNegative".get(), (obj, _) => function(obj, numeric => (KBoolean)numeric.IsNegative));
      registerMessage("isPrimitive".get(), (obj, _) => function(obj, numeric => (KBoolean)numeric.IsPrimitive));
      registerMessage("zfill(_<Int>)",
         (obj, message) => function<IObject, Int>(obj, message, (numeric, i) => ((INumeric)numeric).ZFill(i.Value)));
      registerMessage("negate()", (obj, _) => function(obj, x => -x, x => -x, x => x, x => x.Negate(), "negate()"));
   }

   protected void numericConversionMessages()
   {
      registerMessage("i".get(),
         (obj, _) => function(obj, i => i, d => (int)d, b => b, m => (Int)((INumeric)m).ToInt(), "i".get()));
      registerMessage("f".get(),
         (obj, _) => function(obj, i => i, d => d, b => b, m => (Float)((INumeric)m).ToFloat(), "f".get()));
      registerMessage("b".get(),
         (obj, _) => function(obj, i => (byte)i, d => (byte)d, b => b, m => (KByte)((INumeric)m).ToByte(), "b".get()));
      registerMessage("d".get(), (obj, _) => function(obj, i => i, d => (decimal)d, b => b, m => (KDecimal)((INumeric)m).ToDecimal(), "d".get()));
      registerMessage("im".get(), (obj, _) => Complex.AsImaginary((INumeric)obj));
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
      registerMessage("one()", (obj, _) => collectionFunc(obj, c => c.One()));
      registerMessage("copy()", (obj, _) => collectionFunc(obj, c => c.Copy()));
      registerMessage("following(_)", (obj, msg) => collectionFunc<IObject>(obj, msg, (c, o) => (IObject)c.Following(o)));

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
      registerMessage("isNotEmpty".get(), (obj, _) => function<IObject>(obj, o => ((IMutableCollection)o).IsNotEmpty));
      registerMessage("assign(_,_)", (obj, message) => function<IObject, IObject, IObject>(obj, message, assignToMutable));
      registerMessage("|<<(_)", (obj, message) => function<IObject, IObject>(obj, message, (o, v) => ((IMutableCollection)o).Prepend(v)));
      registerMessage("prepend(_)", (obj, message) => function<IObject, IObject>(obj, message, (o, v) => ((IMutableCollection)o).Prepend(v)));
      registerMessage("clear()", (obj, _) => function<IObject>(obj, o => ((IMutableCollection)o).Clear()));
   }

   protected void loadIteratorMessages()
   {
      dynamicInvoke = (obj, message) =>
      {
         var iterator = (IObject)((ICollection)obj).GetIterator(false);
         return classOf(iterator).SendMessage(iterator, message);
      };
   }

   protected void iteratorMessages()
   {
      alternateMessages.Clear();

      registerIterMessage("collection".get(), (obj, _) => iteratorFunc(obj, i => (IObject)i.Collection));
      registerIterMessage("isLazy".get(), (obj, _) => iteratorFunc(obj, i => (KBoolean)i.IsLazy));
      registerIterMessage("next()", (obj, _) => iteratorFunc(obj, i => i.Next().Map(Some.Object) | (() => KNil.NilValue)));
      registerIterMessage("peek()", (obj, _) => iteratorFunc(obj, i => i.Peek().Map(Some.Object) | (() => KNil.NilValue)));
      registerIterMessage("reset()", (obj, _) => iteratorFunc(obj, i => i.Reset()));
      registerIterMessage("reverse()", (obj, _) => iteratorFunc(obj, i => i.Reverse()));
      registerIterMessage("join()", (obj, _) => iteratorFunc(obj, i => i.Join()));
      registerIterMessage("join(_<String>)", (obj, message) => iteratorFunc<KString>(obj, message, (i, s) => i.Join(s.Value)));
      registerIterMessage("join(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Join(l)));
      registerIterMessage("join(on:_<String>,limit:_<Int>,truncated:_<String>)",
         (obj, msg) => iteratorFunc<KString, Int, KString>(obj, msg, (i, c, l, t) => i.Join(c.Value, l.Value, t.Value)));
      registerIterMessage("join(on:_<String>,limit:_<Int>)",
         (obj, msg) => iteratorFunc<KString, Int>(obj, msg, (i, c, l) => i.Join(c.Value, l.Value, "...")));
      registerIterMessage("join(on:_<String>,prefix:_<String>,suffix:_<String>)",
         (obj, msg) => iteratorFunc<KString, KString, KString>(obj, msg, (i, o, p, s) => i.Join(o.Value, p.Value, s.Value)));
      registerIterMessage("sort(_<Lambda>,asc:_<Boolean>)",
         (obj, message) => iteratorFunc<Lambda, KBoolean>(obj, message, (i, l, b) => i.Sort(l, b.Value)));
      registerIterMessage("sort(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Sort(l, true)));
      registerIterMessage("sortDesc(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Sort(l, false)));
      registerIterMessage("sort(asc:_<Boolean>)", (obj, message) => iteratorFunc<KBoolean>(obj, message, (i, b) => i.Sort(b.Value)));
      registerIterMessage("sort()", (obj, _) => iteratorFunc(obj, i => i.Sort(true)));
      registerIterMessage("sortDesc()", (obj, _) => iteratorFunc(obj, i => i.Sort(false)));
      registerIterMessage("foldl".Selector("_", "_<Lambda>"),
         (obj, message) => iteratorFunc<IObject, Lambda>(obj, message, (i, o, l) => i.FoldLeft(o, l)));
      registerIterMessage("foldl(_)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.FoldLeft(l)));
      registerIterMessage("foldr".Selector("_", "_<Lambda>"),
         (obj, message) => iteratorFunc<IObject, Lambda>(obj, message, (i, o, l) => i.FoldRight(o, l)));
      registerIterMessage("foldr(_)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.FoldRight(l)));
      registerIterMessage("fold".Selector("_", "_<Lambda>"),
         (obj, message) => iteratorFunc<IObject, Lambda>(obj, message, (i, o, l) => i.FoldLeft(o, l)));
      registerIterMessage("reducel".Selector("_", "_<Lambda>"),
         (obj, message) => iteratorFunc<IObject, Lambda>(obj, message, (i, o, l) => i.ReduceLeft(o, l)));
      registerIterMessage("reducel(_)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.ReduceLeft(l)));
      registerIterMessage("reducer".Selector("_", "_<Lambda>"),
         (obj, message) => iteratorFunc<IObject, Lambda>(obj, message, (i, o, l) => i.ReduceRight(o, l)));
      registerIterMessage("reducer(_)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.ReduceRight(l)));
      registerIterMessage("count()", (obj, _) => iteratorFunc(obj, i => i.Count()));
      registerIterMessage("count(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Count(l)));
      registerIterMessage("count(of:_)", (obj, message) => iteratorFunc<IObject>(obj, message, (i, o) => i.Count(o)));
      registerIterMessage("map(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Map(l)));
      registerIterMessage("bind(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Map(l)));
      registerIterMessage("flatMap(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.FlatMap(l)));
      registerIterMessage("if(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.If(l)));
      registerIterMessage("mapAll(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.MapAll(l)));
      registerIterMessage("mapIf(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.MapIf(l)));
      registerIterMessage("ifNot(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.IfNot(l)));
      registerIterMessage("skip(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, j) => i.Skip(j.Value)));
      registerIterMessage("-(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, j) => i.Skip(j.Value)));
      registerIterMessage("skipWhile(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.SkipWhile(l, false)));
      registerIterMessage("skipWhile(back:_<Boolean>,_<Lambda>)",
         (obj, message) => iteratorFunc<KBoolean, Lambda>(obj, message, (i, b, l) => i.SkipWhile(l, b.Value)));
      registerIterMessage("skipUntil(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.SkipUntil(l, false)));
      registerIterMessage("skipUntil(back:_<Boolean>,_<Lambda>)",
         (obj, message) => iteratorFunc<KBoolean, Lambda>(obj, message, (i, b, l) => i.SkipUntil(l, b.Value)));
      registerIterMessage("take(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, j) => i.Take(j.Value)));
      registerIterMessage("+(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, j) => i.Take(j.Value)));
      registerIterMessage("takeWhile(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.TakeWhile(l, false)));
      registerIterMessage("takeWhile(back:_<Boolean>,_<Lambda>)",
         (obj, message) => iteratorFunc<KBoolean, Lambda>(obj, message, (i, b, l) => i.TakeWhile(l, b.Value)));
      registerIterMessage("takeUntil(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.TakeUntil(l, false)));
      registerIterMessage("takeUntil(back:_<Boolean>,_<Lambda>)",
         (obj, message) => iteratorFunc<KBoolean, Lambda>(obj, message, (i, b, l) => i.TakeUntil(l, b.Value)));
      registerIterMessage("index(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Index(l)));
      registerIterMessage("indexes(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Indexes(l)));
      registerIterMessage("zip(_<Collection>,_<Lambda>)",
         (obj, message) => iteratorFunc<IObject, Lambda>(obj, message, (i, c, l) => i.Zip((ICollection)c, l)));
      registerIterMessage("zip(_<Iterator>,_<Lambda>)",
         (obj, message) => iteratorFunc<IObject, Lambda>(obj, message, (i, c, l) => i.Zip((IIterator)c, l)));
      registerIterMessage("zip(_<OpenRange>,_<Lambda>)",
         (obj, message) => iteratorFunc<IObject, Lambda>(obj, message, (i, c, l) => i.Zip((OpenRange)c, l)));
      registerIterMessage("zip(_<NumericOpenRange>,_<Lambda>)",
         (obj, message) => iteratorFunc<IObject, Lambda>(obj, message, (i, c, l) => i.Zip((NumericOpenRange)c, l)));
      registerIterMessage("zip(_<Collection>)", (obj, message) => iteratorFunc<IObject>(obj, message, (i, c) => i.Zip((ICollection)c)));
      registerIterMessage("zip(_<Iterator>)", (obj, message) => iteratorFunc<IObject>(obj, message, (i, c) => i.Zip((IIterator)c)));
      registerIterMessage("zip(_<OpenRange>)", (obj, message) => iteratorFunc<IObject>(obj, message, (i, c) => i.Zip((OpenRange)c)));
      registerIterMessage("zip(_<NumericOpenRange>)", (obj, message) => iteratorFunc<IObject>(obj, message, (i, c) => i.Zip((NumericOpenRange)c)));
      registerIterMessage("zipl(_<Collection>,_,_,_<Lambda>)",
         (obj, message) =>
            iteratorFunc<IObject, IObject, IObject, Lambda>(obj, message, (i, c, lv, rv, l) => i.ZipL((ICollection)c, lv, rv, l)));
      registerIterMessage("zipl(_<Collection>,_,_)",
         (obj, message) => iteratorFunc<IObject, IObject, IObject>(obj, message, (i, c, lv, rv) => i.ZipL((ICollection)c, lv, rv)));
      registerIterMessage("unzip()", (obj, _) => iteratorFunc(obj, i => i.Unzip()));
      registerIterMessage("unzip(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Unzip(l)));
      registerIterMessage("min()", (obj, _) => iteratorFunc(obj, i => i.Min()));
      registerIterMessage("min(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Min(l)));
      registerIterMessage("max()", (obj, _) => iteratorFunc(obj, i => i.Max()));
      registerIterMessage("max(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Max(l)));
      registerIterMessage("first()", (obj, _) => iteratorFunc(obj, i => i.First()));
      registerIterMessage("first".Selector("_<Lambda>"), (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.First(l)));
      registerIterMessage("last()", (obj, _) => iteratorFunc(obj, i => i.Last()));
      registerIterMessage("last".Selector("_<Lambda>"), (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Last(l)));
      registerIterMessage("split(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Split(l)));
      registerIterMessage("split(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, j) => i.Split(j.Value)));
      registerIterMessage("random()", (obj, _) => iteratorFunc(obj, i => i.Random()));
      registerIterMessage("groupBy(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.GroupBy(l)));
      registerIterMessage("groupBy(key:_<Lambda>,value:_<Lambda>)",
         (obj, message) => iteratorFunc<Lambda, Lambda>(obj, message, (i, k, v) => i.GroupBy(k, v)));
      registerIterMessage("one(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.One(l)));
      registerIterMessage("none(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.None(l)));
      registerIterMessage("any(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Any(l)));
      registerIterMessage("all(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.All(l)));
      registerIterMessage("sum()", (obj, _) => iteratorFunc(obj, i => (IObject)i.Sum()));
      registerIterMessage("cumulSum()", (obj, _) => iteratorFunc(obj, i => i.CumulativeSum()));
      registerIterMessage("average()", (obj, _) => iteratorFunc(obj, i => (IObject)i.Average()));
      registerIterMessage("product()", (obj, _) => iteratorFunc(obj, i => (IObject)i.Product()));
      registerIterMessage("cumulProduct()", (obj, _) => iteratorFunc(obj, i => i.CumulativeProduct()));
      registerIterMessage("cross(_)", (obj, message) => iteratorFunc<IObject>(obj, message, (i, c) => i.Cross((ICollection)c)));
      registerIterMessage("cross(_,_)", (obj, message) => iteratorFunc<IObject, Lambda>(obj, message, (i, c, l) => i.Cross((ICollection)c, l)));
      registerIterMessage("by(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, j) => i.By(j.Value)));
      registerIterMessage("/(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, j) => i.By(j.Value)));
      registerIterMessage("window(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, j) => i.Window(j.Value)));
      registerIterMessage("//(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, j) => i.Window(j.Value)));
      registerIterMessage("unique()", (obj, _) => iteratorFunc(obj, i => i.Unique()));
      registerIterMessage("unique(_<Lambda>)", (obj, msg) => iteratorFunc<Lambda>(obj, msg, (i, l) => i.Unique(l)));
      registerIterMessage("span".Selector("_<Lambda>"), (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Span(l)));
      registerIterMessage("span".Selector("_<Int>"), (obj, message) => iteratorFunc<Int>(obj, message, (i, j) => i.Span(j.Value)));
      registerIterMessage("shuffle()", (obj, _) => iteratorFunc(obj, i => i.Shuffle()));
      registerIterMessage("array()", (obj, _) => iteratorFunc(obj, i => i.ToArray()));
      registerIterMessage("list()", (obj, _) => iteratorFunc(obj, i => i.ToList()));
      registerIterMessage("tuple()", (obj, _) => iteratorFunc(obj, i => i.ToTuple()));
      registerIterMessage("dictionary".Selector("key:_<Lambda>", "value:_<Lambda>"),
         (obj, message) => iteratorFunc<Lambda, Lambda>(obj, message, (i, l1, l2) => i.ToDictionary(l1, l2)));
      registerIterMessage("dictionary()", (obj, _) => iteratorFunc(obj, i => i.ToDictionary()));
      registerIterMessage("each(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Each(l)));
      registerIterMessage("rotate(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, c) => i.Rotate(c.Value)));
      registerIterMessage("shift(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, c) => i.Shift(c.Value)));
      registerIterMessage("shift(_<Int>,default:_)", (obj, message) => iteratorFunc<Int, IObject>(obj, message, (i, c, d) => i.Shift(c.Value, d)));
      registerIterMessage("permutations(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, c) => i.Permutations(c.Value)));
      registerIterMessage("permutations()", (obj, _) => iteratorFunc(obj, i => i.Permutations()));
      registerIterMessage("combinations(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, c) => i.Combinations(c.Value)));
      registerIterMessage("combinations()", (obj, _) => iteratorFunc(obj, i => i.Combinations()));
      registerIterMessage("flatten()", (obj, _) => iteratorFunc(obj, i => i.Flatten()));
      registerIterMessage("copy()", (obj, _) => iteratorFunc(obj, i => i.Copy()));
      registerIterMessage("collect()", (obj, _) => iteratorFunc(obj, i => i.Collect()));
      registerIterMessage("*(_)", (obj, message) => iteratorFunc<IObject>(obj, message, (i1, i2) => i1.Apply((ICollection)i2)));
      //registerIterMessage("format(_)", (obj, message) => iteratorFunc<KIndex>(obj, message, (i, index) => index.GetFromCollection(i.Collection)));
      registerIterMessage("replace(_<Lambda>,_<Lambda>)",
         (obj, message) => iteratorFunc<Lambda, Lambda>(obj, message, (i, l1, l2) => i.Replace(l1, l2)));
      registerIterMessage("set()", (obj, _) => iteratorFunc(obj, i => i.ToSet()));
      registerIterMessage("shape(_<Int>,_<Int>)",
         (obj, message) => iteratorFunc<Int, Int>(obj, message, (i, j, k) => i.Shape(j.Value, k.Value)));
      registerIterMessage("%(_<Tuple>)",
         (obj, msg) => iteratorFunc<KTuple>(obj, msg, (i, t) => i.Shape(((Int)t.Value[0]).Value, ((Int)t.Value[1]).Value)));
      registerIterMessage("shape(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, j) => i.Shape(0, j.Value)));
      registerIterMessage("%(_<Int>)", (obj, msg) => iteratorFunc<Int>(obj, msg, (i, cols) => i.Shape(0, cols.Value)));
      registerIterMessage("column(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, c) => i.Column(c.Value)));
      registerIterMessage("partition(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Partition(l)));
      registerIterMessage("pick(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, c) => i.Pick(c.Value)));
      registerIterMessage("pick()", (obj, _) => iteratorFunc(obj, i => i.Pick()));
      registerIterMessage("roll(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, c) => i.Roll(c.Value)));
      registerIterMessage("splat(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, c) => i.Splat(c.Value)));
      registerIterMessage("chunked(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, c) => i.Chunked(c.Value)));
      registerIterMessage("windowed(size:_<Int>,step:_<Int>)",
         (obj, message) => iteratorFunc<Int, Int>(obj, message, (i, s1, s2) => i.Windowed(s1.Value, s2.Value, true)));
      registerIterMessage("windowed(size:_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, s) => i.Windowed(s.Value, 1, true)));
      registerIterMessage("windowed(size:_<Int>,step:_<Int>,partial:_<Boolean>)",
         (obj, message) => iteratorFunc<Int, Int, KBoolean>(obj, message, (i, s1, s2, p) => i.Windowed(s1.Value, s2.Value, p.Value)));
      registerIterMessage("repeated()", (obj, _) => iteratorFunc(obj, i => i.Repeated()));
      registerIterMessage("accumulate(_<Lambda>)", (obj, message) => iteratorFunc<Lambda>(obj, message, (i, l) => i.Accumulate(l)));
      registerIterMessage("accumulate(init:_,_<Lambda>)",
         (obj, message) => iteratorFunc<IObject, Lambda>(obj, message, (i, v, l) => i.Accumulate(v, l)));
      registerIterMessage("allTrue(_)", (obj, message) => iteratorFunc<IObject>(obj, message, (i, c) => i.AllTrue(c)));
      registerIterMessage("anyTrue(_)", (obj, message) => iteratorFunc<IObject>(obj, message, (i, c) => i.AnyTrue(c)));
      registerIterMessage("noneTrue(_)", (obj, message) => iteratorFunc<IObject>(obj, message, (i, c) => i.NoneTrue(c)));
      registerIterMessage("headTail()", (obj, _) => iteratorFunc(obj, i => i.HeadTail()));
      registerIterMessage("junctionAll()", (obj, _) => iteratorFunc(obj, i => i.JunctionAll()));
      registerIterMessage("junctionAny()", (obj, _) => iteratorFunc(obj, i => i.JunctionAny()));
      registerIterMessage("junctionNone()", (obj, _) => iteratorFunc(obj, i => i.JunctionNone()));
      registerIterMessage("junctionOne()", (obj, _) => iteratorFunc(obj, i => i.JunctionOne()));
      registerIterMessage("step(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, c) => i.Step(c.Value)));
      registerIterMessage("[](_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i, c) => i[c.Value]));
      registerIterMessage("seq()", (obj, _) => iteratorFunc(obj, i => i.Seq()));
      registerIterMessage("transpose()", (obj, _) => iteratorFunc(obj, i => i.Transpose()));
      registerIterMessage("assoc(_)", (obj, message) => iteratorFunc<IObject>(obj, message, (i, c) => i.Assoc(c)));
      registerIterMessage("at(_<Int>)", (obj, message) => iteratorFunc<Int>(obj, message, (i1, i2) => i1.At(i2.Value)));
      registerIterMessage("dotProduct(_<Collection>)",
         (obj, message) => iteratorFunc<IObject>(obj, message, (i1, i2) => i1.DotProduct((ICollection)i2)));
   }

   public void typedCollectionMessages()
   {
      registerMessage("setType(_<TypeConstraint>)", (obj, msg) => ((ITypedCollection)obj).SetType((TypeConstraint)msg.Arguments[0]));
      registerMessage("autoType()", (obj, _) => ((ITypedCollection)obj).AutoType());
   }

   public virtual bool MatchCompatible(BaseClass otherClass) => Name == otherClass.Name;

   public virtual bool AssignCompatible(BaseClass otherClass) =>
      otherClass.Name is "Placeholder" or "Undefined" or "Any" || MatchCompatible(otherClass);

   protected void rangeMessages()
   {
      registerMessage("succ".get(), (obj, _) => function<IObject>(obj, o => (IObject)((IRangeItem)o).Successor));
      registerMessage("pred".get(), (obj, _) => function<IObject>(obj, o => (IObject)((IRangeItem)o).Predecessor));
      registerMessage("range()", (obj, _) => function<IObject>(obj, o => ((IRangeItem)o).Range()));
   }

   protected void indexedMessages()
   {
      registerMessage("start".get(), (obj, _) => function<IObject>(obj, i => ((IIndexed)i).Start));
      registerMessage("end".get(), (obj, _) => function<IObject>(obj, i => ((IIndexed)i).End));
      registerMessage("indexes".get(), (obj, _) => function<IObject>(obj, i => ((IIndexed)i).Indexes));
      registerMessage("[](_<Range>)", (obj, msg) => function<IObject, KRange>(obj, msg, (o, i) => ((IIndexed)o)[i]));
      registerMessage("[]=(_<Range>,_)", (obj, msg) => function<IObject, KRange, IObject>(obj, msg, (o, i, v) => ((IIndexed)o)[i] = v));
   }

   public static IObject Invoke(IObject obj, Arguments arguments, Lambda lambda, bool bareLambda)
   {
      Fields fields;
      if (obj is UserObject uo)
      {
         fields = uo.Fields;
      }
      else
      {
         fields = new Fields();
         fields.New("self", FieldType.Assignment);
         fields.Assign("self", obj);
      }

      var _value = Machine.Current.Invoke(lambda.Invokable, arguments, fields, bareLambda);
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

   public static IObject Invoke(UserClass userClass, Arguments arguments, Lambda lambda, bool bareLambda)
   {
      return Machine.Current.Invoke(lambda.Invokable, arguments, userClass.ClassFields, bareLambda)
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
      registerMessage("round(_<Int>)", (obj, message) => msgNumberFunction(obj, message, (x, y) => x.Round(y)));
   }

   protected void sliceableMessages()
   {
      registerMessage("slice(_)",
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
      registerMessage("<>(_)", (o, m) => (Int)((IObjectCompare)o).Compare(m.Arguments[0]));
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
      messages["index(_<Lambda>)"] = (obj, msg) => ((IFindIndex)obj).Index((Lambda)msg.Arguments[0]);
      messages["lastIndex(of:_)"] = (obj, msg) => ((IFindIndex)obj).LastIndexOf(msg.Arguments[0]);
      messages["lastIndex(_<Lambda>)"] = (obj, msg) => ((IFindIndex)obj).LastIndex((Lambda)msg.Arguments[0]);
      messages["find(all:_<Lambda>)"] = (obj, msg) => ((IFindIndex)obj).FindAll((Lambda)msg.Arguments[0]);
      messages["first(_<Lambda>)"] = (obj, msg) => ((IFindIndex)obj).First((Lambda)msg.Arguments[0]);
      messages["last(_<Lambda>)"] = (obj, msg) => ((IFindIndex)obj).Last((Lambda)msg.Arguments[0]);
      messages["binarySearch(_)"] = (obj, msg) => ((IFindIndex)obj).BinarySearch(msg.Arguments[0]);
      messages["binarySearch(_,_<Lambda>)"] = (obj, msg) => ((IFindIndex)obj).BinarySearch(msg.Arguments[0], (Lambda)msg.Arguments[1]);
   }

   protected void acceptingMessages()
   {
      RegisterMessage("accept(_)", (obj, msg) => ((IAccepting)obj).Accept(msg.Arguments[0]));
   }

   public void RegisterInclusion(Inclusion inclusion) => inclusions[inclusion.Name] = inclusion;

   public bool Includes(string inclusionName) => inclusions.ContainsKey(inclusionName);

   public abstract IObject DefaultValue { get; }

   public bool Equals(BaseClass? other) => other is not null && Name == other.Name;

   public override bool Equals(object? obj) => obj is BaseClass baseClass && Equals(baseClass);

   public override int GetHashCode() => Name.GetHashCode();

   public static bool operator ==(BaseClass? left, BaseClass? right) => Equals(left, right);

   public static bool operator !=(BaseClass? left, BaseClass? right) => !Equals(left, right);
}