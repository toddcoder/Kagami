﻿using System;
using Kagami.Library.Objects;
using static Kagami.Library.Operations.NumericFunctions;

namespace Kagami.Library.Classes
{
   public class FloatClass : BaseClass
   {
      public override string Name => "Float";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         numericMessages();
         numericConversionMessages();
         formatMessage<Float>();
         compareMessages();

         messages["round"] = (obj, msg) => function(obj, msg, (a, b) => Math.Round(a, (int)b), (a, b) => a.Round(b), "round");
      }

      public override void RegisterClassMessages()
      {
         base.RegisterClassMessages();

         classMessages["e".get()] = (cls, msg) => (Float)Math.E;
         classMessages["pi".get()] = (cls, msg) => (Float)Math.PI;
         classMessages["nan".get()] = (cls, msg) => (Float)double.NaN;
         classMessages["parse"] = (cls, msg) => Float.Object(double.Parse(msg.Arguments[0].AsString));
      }
   }
}