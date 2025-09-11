using System.Globalization;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Classes.ClassFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Classes;

public class IntClass : BaseClass, IParse, IEquivalentClass
{
   protected Lazy<Random> random = new(() => new Random(DateTime.Now.Microsecond));

   public override string Name => "Int";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      numericMessages();
      numericConversionMessages();
      rangeMessages();
      compareMessages();

      messages["isEven".get()] = (obj, _) => function<Int>(obj, i => i.IsEven);
      messages["isOdd".get()] = (obj, _) => function<Int>(obj, i => i.IsOdd);
      messages["isPrime".get()] = (obj, _) => function<Int>(obj, i => i.IsPrime);
      messages["factorial()"] = (obj, _) => function<Int>(obj, i => i.Factorial());
      messages["millisecond".get()] = (obj, _) => function<Int>(obj, i => i.Millisecond);
      messages["milliseconds".get()] = (obj, _) => function<Int>(obj, i => i.Millisecond);
      messages["second".get()] = (obj, _) => function<Int>(obj, i => i.Second);
      messages["seconds".get()] = (obj, _) => function<Int>(obj, i => i.Second);
      messages["minute".get()] = (obj, _) => function<Int>(obj, i => i.Minute);
      messages["minutes".get()] = (obj, _) => function<Int>(obj, i => i.Minute);
      messages["hour".get()] = (obj, _) => function<Int>(obj, i => i.Hour);
      messages["hours".get()] = (obj, _) => function<Int>(obj, i => i.Hour);
      messages["day".get()] = (obj, _) => function<Int>(obj, i => i.Day);
      messages["days".get()] = (obj, _) => function<Int>(obj, i => i.Day);
      messages["week".get()] = (obj, _) => function<Int>(obj, i => i.Week);
      messages["weeks".get()] = (obj, _) => function<Int>(obj, i => i.Week);
      messages["char()"] = (obj, _) => function<Int>(obj, i => i.Char());
      messages["byte()"] = (obj, _) => function<Int>(obj, i => i.Byte());
      messages["times(_)"] = (obj, msg) => function<Int, Lambda>(obj, msg, (i, l) => i.Times(l));
      messages["<<(_)"] = (obj, msg) => function<Int, IObject>(obj, msg, (i, o) => i.ShiftLeft(o));
      messages[">>(_)"] = (obj, msg) => function<Int, IObject>(obj, msg, (i, o) => i.ShiftRight(o));
      messages["nextPrime()"] = (obj, _) => function<Int>(obj, i => i.NextPrime());
      messages["max(_<Int>)"] = (obj, msg) => function<Int, Int>(obj, msg, (i1, i2) => i1.Max(i2));
      messages["min(_<Int>)"] = (obj, msg) => function<Int, Int>(obj, msg, (i1, i2) => i1.Min(i2));
      messages["rand()"] = (obj, _) => function<Int>(obj, i => i.Rand(random.Value));
      messages["rand(_<Int>)"] = (obj, msg) => function<Int, Int>(obj, msg, (i1, i2) => i1.Rand(random.Value, i2));
      messages["base(_<Int>)"] = (obj, msg) => function<Int, Int>(obj, msg, (i1, i2) => i1.Base(i2.Value));
   }

   public override void RegisterClassMessages()
   {
      base.RegisterClassMessages();

      classMessages["min".get()] = (_, _) => Int.IntObject(int.MinValue);
      classMessages["max".get()] = (_, _) => Int.IntObject(int.MaxValue);
      classMessages["parse(_)"] = (_, msg) => parse(msg.Arguments[0].AsString);
      classMessages["parse(hex:_)"] = (_, msg) => parseFromHex(msg.Arguments[0].AsString);
      classMessages["parse(bin:_)"] = (_, msg) => parseFromBinary(msg.Arguments[0].AsString);
      classMessages["parse(_<String>,radix:_<Int>)"] = (_, msg) =>
         classFunc<IntClass, KString, Int>(this, msg, (_, s, r) => successOf(parseInt(s.Value, r.Value).Map(Int.IntObject)));
      classMessages["rand()"] = (_, _) => (Int)random.Value.Next();
   }

   public static IObject parse(string value)
   {
      try
      {
         var number = int.Parse(value.Replace("_", ""));
         return Success.Object(Int.IntObject(number));
      }
      catch (Exception exception)
      {
         return Failure.Object(exception.Message);
      }
   }

   public static IObject parseFromNumberStyles(string value, NumberStyles numberStyles)
   {
      try
      {
         var number = int.Parse(value.Replace("_", ""), numberStyles);
         return Success.Object(Int.IntObject(number));
      }
      catch (Exception exception)
      {
         return Failure.Object(exception.Message);
      }
   }

   public static IObject parseFromHex(string value) => parseFromNumberStyles(value.Drop("^ '0x'"), NumberStyles.AllowHexSpecifier);

   public static IObject parseFromBinary(string value) => parseFromNumberStyles(value.Drop("^ '0b'"), NumberStyles.AllowBinarySpecifier);

   public IObject Parse(string source) => Int.IntObject(source.Value().Int32());

   public override bool IsNumeric => true;

   public override IObject DefaultValue => Int.Zero;

   public TypeConstraint EquivalentTypeConstraint() => TypeConstraint.FromList("Number");

   protected static Result<int> parseInt(string source, int radix)
   {
      var x = 0;

      foreach (var _digit in source.Select(charToDigit))
      {
         if (_digit is (true, var digit))
         {
            if (digit >= radix)
            {
               return fail("Invalid char");
            }

            x *= radix;
            x += digit;
         }
         else
         {
            return fail("Char out of range");
         }
      }

      return x;

      static Maybe<int> charToDigit(char c) => c switch
      {
         >= '0' and <= '9' => c - '0',
         >= 'A' and <= 'Z' => c - 'A' + 10,
         >= 'a' and <= 'z' => c - 'a' + 10,
         _ => nil
      };
   }
}