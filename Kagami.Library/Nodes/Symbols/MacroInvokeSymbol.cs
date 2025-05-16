using Kagami.Library.Nodes.Statements;
using Kagami.Library.Operations;
using Core.Enumerables;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class MacroInvokeSymbol : Symbol
{
   protected Function function;
   protected Expression[] arguments;

   public MacroInvokeSymbol(Function function, Expression[] arguments)
   {
      this.function = function;
      this.arguments = arguments;
   }

   public override void Generate(OperationsBuilder builder)
   {
      var block = function.Block;
      var _expression = block.ExpressionStatement(true);
      if (_expression is (true, var expression))
      {
         builder.BeginMacro(function.Parameters, arguments);
         expression.Generate(builder);
      }
      else
      {
         var label = newLabel("return");
         builder.BeginMacro(function.Parameters, arguments, label);
         block.Generate(builder);

         builder.Label(label);
         builder.NoOp();
      }

      builder.EndMacro();
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"{function.Selector}({arguments.ToString(", ")})";
}