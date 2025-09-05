using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class NewObjectSymbol : Symbol, IHasExpressions
{
   protected string tempObjectField;
   protected string className;
   protected TaggedExpression[] taggedExpressions;

   public NewObjectSymbol(string tempObjectField, string className, TaggedExpression[] taggedExpressions)
   {
      this.tempObjectField = tempObjectField;
      this.className = className;
      this.taggedExpressions = taggedExpressions;
   }

   public override void Generate(OperationsBuilder builder)
   {
      builder.NewField(tempObjectField, false, true);
      builder.Invoke(className, 0);
      builder.AssignField(tempObjectField, false);
      foreach (var (tag, expression) in taggedExpressions)
      {
         builder.Dup();
         expression.Generate(builder);
         builder.SendMessage(tag.set(), 1);
         //builder.Drop();
      }
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"let {tempObjectField} = {className}() {taggedExpressions}";

   public Expression[] Expressions => [.. taggedExpressions.Select(te => te.Expression)];
}