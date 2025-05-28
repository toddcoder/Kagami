using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class BindingSymbol : Symbol
{
   protected string name;
   protected Symbol value;

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
         var s = ((KString)t[0]).Value;
         var o = t[1];
         return new Binding(s, o);
      });
   }

   public override Precedence Precedence => Precedence.KeyValue;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => $"{name} @ {value}";
}