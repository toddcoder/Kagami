using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class NewObjectSymbol : Symbol
   {
      protected string tempObjectField;
      protected string className;
      protected Expression expression;

      public NewObjectSymbol(string tempObjectField, string className, Expression expression)
      {
         this.tempObjectField = tempObjectField;
         this.className = className;
         this.expression = expression;
      }

      public override void Generate(OperationsBuilder builder)
      {
         builder.NewField(tempObjectField, false, true);
         builder.Invoke(className, 0);
         builder.AssignField(tempObjectField, false);
         expression.Generate(builder);
         builder.GetField(tempObjectField);
      }

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => $"let {tempObjectField} = {className}() {expression}";
   }
}