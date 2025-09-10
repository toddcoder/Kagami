using System.Numerics;
using Core.Matching;
using Core.Monads;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Classes.ClassFunctions;
using static Kagami.Library.CommonFunctions;

namespace Kagami.Library.Classes;

public class RationalClass : BaseClass, IEquivalentClass
{
   public override string Name => "Rational";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      messageNumberMessages();
      compareMessages();
      numericConversionMessages();

      registerMessage("numerator".get(), (obj, _) => function<Rational>(obj, r => Long.LongObject(r.Numerator)));
      registerMessage("denominator".get(), (obj, _) => function<Rational>(obj, r => Long.LongObject(r.Denominator)));
   }

   public override void RegisterClassMessages()
   {
      base.RegisterClassMessages();

      registerClassMessage("parse(_)", (bc, msg) => classFunc<RationalClass, KString>(bc, msg, (_, s) => resultToSuccess(Parse(s.Value))));
   }

   public static Result<Rational> Parse(string source)
   {
      try
      {
         var _result = source.Matches("^ /(/d [/d '_`']*) 'L'? /s* '//'1%2 /s* /(/d [/d '_`']*) 'L'? $");
         if (_result is (true, var result))
         {
            var numeratorSource = result.FirstGroup;
            var denominatorSource = result.SecondGroup;
            if (!BigInteger.TryParse(numeratorSource, out var numerator))
            {
               return fail($"{numeratorSource} can't be converted to a Long");
            }

            if (!BigInteger.TryParse(denominatorSource, out var denominator))
            {
               return fail($"{denominatorSource} can't be converted to a Long");
            }

            return new Rational(numerator, denominator);
         }
         else
         {
            return fail($"{source} isn't in a proper format for a Rational");
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public override bool IsNumeric => true;

   public override IObject DefaultValue => new Rational(0, 1);

   public TypeConstraint EquivalentTypeConstraint() => TypeConstraint.FromList("Number");
}