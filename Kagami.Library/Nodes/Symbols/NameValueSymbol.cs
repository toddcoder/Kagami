﻿using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class NameValueSymbol : Symbol
   {
      string name;
      Expression value;

      public NameValueSymbol(string name, Expression value)
      {
         this.name = name;
         this.value = value;
      }

      public override void Generate(OperationsBuilder builder)
      {
         builder.PushString(name);
         value.Generate(builder);
         builder.ToArguments(2);
         builder.NewValue("NameValue", t =>
         {
            var oname = ((String)t[0]).Value;
            var ovalue = t[1];
            return new NameValue(oname, ovalue);
         });
      }

      public override Precedence Precedence => Precedence.KeyValue;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => $"{name}: {value}";
   }
}