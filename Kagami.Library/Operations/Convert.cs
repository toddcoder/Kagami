using System.Numerics;
using Core.Collections;
using Core.Monads;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using Arguments = Kagami.Library.Objects.Arguments;

namespace Kagami.Library.Operations;

public class Convert : Operation
{
   protected bool increment = true;

   protected static readonly Hash<(string from, string to), Func<IObject, IObject>> conversions = [];

   static Convert()
   {
      conversions[("Float", "Int")] = f => Int.IntObject(((Float)f).AsInt32());
      conversions[("Int", "Byte")] = f => KByte.ByteObject(((Int)f).AsByte());
      conversions[("Float", "Byte")] = f => KByte.ByteObject(((Float)f).AsByte());
      conversions[("Long", "Int")] = l => Int.IntObject(((Long)l).AsInt32());
      conversions[("String", "Int")] = s => Int.IntObject(int.Parse(s.AsString));
      conversions[("String", "Float")] = s => Float.FloatObject(double.Parse(s.AsString));
      conversions[("String", "Long")] = s => Long.LongObject(BigInteger.Parse(s.AsString));
      conversions[("String", "Byte")] = s => KByte.ByteObject(byte.Parse(s.AsString));
      conversions[("Float", "Rational")] = f => Rational.RationalObject(((Float)f).AsRational());
      conversions[("Int", "Rational")] = i => Rational.RationalObject(((Int)i).AsRational());
      conversions[("Long", "Rational")] = l => Rational.RationalObject(((Long)l).AsRational());
      conversions[("Char", "Byte")] = c => KByte.ByteObject((byte)((KChar)c).Value);
   }

   public override Optional<IObject> Execute(Machine machine)
   {
      var _arguments =
         from toClassValue in machine.Pop()
         from fromClassValue in machine.Pop()
         from valueToConvert in machine.Pop()
         select (valueToConvert, fromClassValue, toClassValue);
      if (_arguments is (true, var (value, fromClass, toClass)))
      {
         var _selector = Module.Global.Value.GetConversion(fromClass.AsString, toClass.AsString);
         if (_selector is (true, var selector))
         {
            var _field = machine.Find(selector);
            if (_field is (true, var field))
            {
               var arguments = new Arguments(value);
               return Invoke.InvokeObject(machine, field.Value, arguments, ref increment);
            }
            else
            {
               return fieldNotFound(selector);
            }
         }
         else if (Module.AutoConversion(fromClass.AsString, toClass.AsString) is (true, var implicitConverter))
         {
            try
            {
               return implicitConverter(value).Just();
            }
            catch (Exception exception)
            {
               return exception;
            }
         }
         else if (conversions.Maybe[(fromClass.AsString, toClass.AsString)] is (true, var converter))
         {
            try
            {
               return converter(value).Just();
            }
            catch (Exception exception)
            {
               return exception;
            }
         }
         else if (fromClass.AsString == "Array" && toClass.AsString == "String")
         {
            return fromCharArrayToString(value);
         }
         else if (toClass.AsString == "Optional")
         {
            return value switch
            {
               Objects.Some or KNil => value.Just(),
               Objects.Success success => Objects.Some.Object(success.Value).Just(),
               Objects.Failure => KNil.NilValue.Just(),
               _ => Objects.Some.Object(value).Just()
            };
         }
         else
         {
            return simpleConversions(fromClass.AsString, toClass.AsString, value);
         }
      }
      else
      {
         return emptyStack("convert");
      }

      static Optional<IObject> fromCharArrayToString(IObject value)
      {
         var auto = (KArray)((KArray)value).AutoType();
         if (auto.TypeConstraint is (true, { Comparisands: [CharClass] }))
         {
            return new KString(new string([.. auto.List.Select(i => (KChar)i).Select(c => c.Value)]));
         }
         else
         {
            return fail("Must be an array of Char");
         }
      }

      static Optional<IObject> simpleConversions(string fromClassName, string toClassName, IObject value)
      {
         return toClassName switch
         {
            "Boolean" => KBoolean.BooleanObject(value.IsTrue).Just(),
            "String" => KString.StringObject(value.AsString).Just(),
            _ => fail($"Conversion from {fromClassName} to {toClassName} not found")
         };
      }
   }

   public override bool Increment => increment;

   public override string ToString() => "convert";
}