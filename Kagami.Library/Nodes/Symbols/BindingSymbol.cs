using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class BindingSymbol : Symbol
   {
      string name;
      Symbol value;

      public BindingSymbol(string name, Symbol value)
      {
         this.name = name;
         this.value = value;
      }

      public override void Generate(OperationsBuilder builder)
      {
         builder.PushString(name);
         value.Generate(builder);
         builder.ToArguments(2);

         builder.NewValue("Binding", t =>
         {
            var oname = ((String)t[0]).Value;
            var ovalue = t[1];
            return new Binding(oname, ovalue);
         });
      }

      public override Precedence Precedence => Precedence.KeyValue;


      public override Arity Arity => Arity.Binary;

      public override string ToString() => $"{name} @ {value}";
   }
}