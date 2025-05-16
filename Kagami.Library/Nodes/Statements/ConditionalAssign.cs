using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using Core.Monads;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Statements;

public class ConditionalAssign : Statement
{
   protected Symbol comparisand;
   protected Expression expression;
   protected Block block;
   protected Maybe<Block> _elseBlock;

   public ConditionalAssign(Symbol comparisand, Expression expression, Block block, Maybe<Block> _elseBlock)
   {
      this.comparisand = comparisand;
      this.expression = expression;
      this.block = block;
      this._elseBlock = _elseBlock;
   }

   public override void Generate(OperationsBuilder builder)
   {
      var elseLabel = newLabel("else");
      var endLabel = newLabel("end");

      builder.PushFrame();
      expression.Generate(builder);
      comparisand.Generate(builder);
      builder.Match();
      builder.GoToIfFalse(_elseBlock ? elseLabel : endLabel);

      builder.PushFrame();
      block.Generate(builder);
      builder.PopFrame();
      builder.GoTo(endLabel);

      builder.Label(elseLabel);
      if (_elseBlock is (true, var elseBlock))
      {
         builder.PushFrame();
         elseBlock.Generate(builder);
         builder.PopFrame();
      }

      builder.Label(endLabel);
      builder.PopFrame();
   }

   public override string ToString() => "";
}