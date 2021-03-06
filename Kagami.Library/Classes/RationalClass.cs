﻿using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
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

      public override bool IsNumeric => true;

      public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Number");
   }
}