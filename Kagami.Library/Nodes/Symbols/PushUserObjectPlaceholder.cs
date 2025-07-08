using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class PushUserObjectPlaceholder(UserObjectPlaceholder userObjectPlaceholder, Expression[] arguments) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.PushObject(userObjectPlaceholder);

      foreach (var expression in arguments)
      {
         expression.Generate(builder);
      }

      builder.ToArguments(arguments.Length);
      builder.RunTimeArguments();
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => userObjectPlaceholder.AsString;
}