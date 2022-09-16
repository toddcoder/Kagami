using System.Linq;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Core.Enumerables;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols
{
   public class InitializeSymbol : Symbol
   {
      protected string className;
      protected (string name, Expression value)[] arguments;

      public InitializeSymbol(string className, (string, Expression)[] arguments)
      {
         this.className = className;
         this.arguments = arguments;
      }

      public override void Generate(OperationsBuilder builder)
      {
         var objectField = newLabel("object");
         builder.NewField(objectField, false, true);
         builder.Invoke(className, 0);
         builder.AssignField(objectField, true);

         foreach (var (name, value) in arguments)
         {
            builder.GetField(objectField);
            Selector message = $"__${name}=(_)";
            value.Generate(builder);
            builder.SendMessage(message, 1);
         }

         builder.GetField(objectField);
      }

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => $"{className}{{{arguments.Select(t => $"{t.name}: {t.value}").ToString(", ")}}}";
   }
}