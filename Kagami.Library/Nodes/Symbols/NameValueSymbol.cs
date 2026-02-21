using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class NameValueSymbol : Symbol, IHasExpression
{
   protected string name;
   protected Expression value;

   public NameValueSymbol(string name, Expression value)
   {
      this.name = name;
      this.value = value;
   }

   public override void Generate(OperationsBuilder builder)
   {
      builder.PushString(name);
      value.Generate(builder);
      builder.NewNameValue();
   }

   public override Precedence Precedence => Precedence.KeyValue;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => $"{name}: {value}";

   public (string, Expression) Tuple() => (name, value);

   public void Deconstruct(out string name, out Expression value)
   {
      name = this.name;
      value = this.value;
   }

   public Expression Expression => value;
}