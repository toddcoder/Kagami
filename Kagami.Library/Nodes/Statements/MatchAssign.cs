using Core.Monads;
using Core.Strings;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Statements;

public class MatchAssign : Statement
{
   protected Expression comparisand;
   protected Expression expression;
   protected Maybe<Block> _block;

   public MatchAssign(Expression comparisand, Expression expression, Maybe<Block> _block)
   {
      this.comparisand = comparisand;
      this.expression = expression;
      this._block = _block;
   }

   public override void Generate(OperationsBuilder builder)
   {
      var fieldName = Guid.NewGuid().ToString();
      builder.NewField(fieldName, false, true);
      expression.Generate(builder);
      builder.Peek(Index);
      builder.AssignField(fieldName, false);
      comparisand.Generate(builder);
      builder.GetField(fieldName);
      builder.Swap();
      builder.Match();

      if (_block is (true, var block))
      {
         foreach (var placeholderName in placeholdersFromExpression(comparisand).Distinct())
         {
            string name;
            var mutable = false;
            var create = true;
            if (placeholderName.StartsWith('+'))
            {
               name = placeholderName.Drop(1);
               mutable= true;
            }
            else if (placeholderName.StartsWith('-'))
            {
               name = placeholderName.Drop(1);
            }
            else
            {
               name = placeholderName;
               create = false;
            }

            if (create)
            {
               builder.NewField(name, mutable, true);
            }
         }

         var end = newLabel("end");
         builder.GoToIfTrue(end);
         block.Generate(builder);
         builder.Label(end);
         builder.NoOp();
      }
      else
      {
         builder.Drop();
      }
   }

   public override string ToString() => $"when {comparisand} = {expression}" + (_block.Map(b => $" else {{{b}}}") | "");
}