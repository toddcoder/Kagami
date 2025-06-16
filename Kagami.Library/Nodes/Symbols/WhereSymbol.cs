using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class WhereSymbol : Symbol
{
   protected (string propertyName, Expression comparisand)[] items;

   public WhereSymbol((string, Expression)[] items) => this.items = items;

   public override void Generate(OperationsBuilder builder)
   {
      var labelFalse = newLabel("false");
      var labelEnd = newLabel("end");
      var fieldName = newLabel("subject");

      builder.NewField(fieldName, false, true);
      builder.AssignField(fieldName, true);

      foreach (var (propertyName, comparisand) in items)
      {
         var getter = propertyName.get();
         builder.GetField(fieldName);
         builder.SendMessage(getter, 0);
         comparisand.Generate(builder);
         builder.Match();
         builder.GoToIfFalse(labelFalse);
      }

      builder.PushBoolean(true);
      builder.GoTo(labelEnd);

      builder.Label(labelFalse);
      builder.PushBoolean(false);

      builder.Label(labelEnd);
   }

   public override Precedence Precedence => Precedence.SendMessage;

   public override Arity Arity => Arity.Postfix;

   public override string ToString() => $"?{{{items.Select(i => $"{i.propertyName}: {i.comparisand}")}}}";
}